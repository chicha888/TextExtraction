using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;
using System.Text;
using Tesseract;
using TextExtraction.Interfaces;
using TextExtraction.Models;

namespace TextExtraction.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly string _tessdataPath;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
            _logger = logger;
            _environment = environment;
            _tessdataPath = Path.Combine(_environment.ContentRootPath, "AppData", "tessdata");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtractText(List<IFormFile> uploadedImages)
        {
            if (uploadedImages == null || uploadedImages.Count == 0)
            {
                TempData["ErrorMessage"] = "Please upload at least one valid image.";
                return RedirectToAction("Index");
            }

            try
            {
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var recognizedTexts = new List<string>();

                foreach (var uploadedImage in uploadedImages)
                {
                    if (uploadedImage.Length > 0)
                    {
                        var filePath = Path.Combine(uploadPath, uploadedImage.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedImage.CopyToAsync(stream);
                        }

                        // ������������ ����������� � ��������� �����
                        var extractedText = ProcessAndExtractText(filePath);
                        recognizedTexts.Add(extractedText);

                        // ��������� � ���� ������, ���� ������������ �����������
                        if (User.Identity.IsAuthenticated)
                        {
                            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                            if (userId != null)
                            {
                                var fileHistory = new FileHistory
                                {
                                    UserId = userId,
                                    FileName = uploadedImage.FileName,
                                    UploadedAt = DateTime.UtcNow,
                                    ExtractedText = extractedText
                                };

                                _historyRepository.Add(fileHistory);
                            }
                        }
                    }
                }

                // ���������� ������ � �������������
                ViewBag.ExtractedTexts = recognizedTexts;
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private string ProcessAndExtractText(string imagePath)
        {
            // ��������� �����������
            Mat img = Cv2.ImRead(imagePath, ImreadModes.Color);

            // ����������� ����������� � ������� ������
            Mat gray = new Mat();
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);

            // ��������� ��������� ����������� (����� OTSU)
            Mat thresh = new Mat();
            Cv2.Threshold(gray, thresh, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);

            // ������� ����������� ������� ��� ��������������� ���������
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(18, 18));

            // ��������� ��������� (���������� ��������)
            Mat dilated = new Mat();
            Cv2.Dilate(thresh, dilated, kernel);

            // ������� �������
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(dilated, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

            // ������� ������ ��� ����������
            string recognizedText = "";

            // ������������ ������ ������
            foreach (var contour in contours)
            {
                // ������� �������������� ������������� ��� �������
                OpenCvSharp.Rect boundingBox = Cv2.BoundingRect(contour);

                // �������� ��������� ����
                Mat cropped = new Mat(img, boundingBox);

                // ����������� ���������� ����� � ������, ���������� ��� Tesseract
                string tempImagePath = Path.Combine(Path.GetTempPath(), "temp_cropped.png");
                Cv2.ImWrite(tempImagePath, cropped);

                // ��������� ����� � ������� Tesseract
                using (var engine = new TesseractEngine(_tessdataPath, "eng+rus+fra+deu+spa+ita+chi_sim+jpn+ara+por", EngineMode.Default))
                {
                    using (var pix = Pix.LoadFromFile(tempImagePath))
                    {
                        using (var page = engine.Process(pix))
                        {
                            recognizedText += page.GetText() + Environment.NewLine;
                        }
                    }
                }
            }

            return recognizedText;
        }

        [HttpGet]
        public IActionResult DownloadText()
        {
            if (TempData["ExtractedText"] != null)
            {
                string extractedText = TempData["ExtractedText"].ToString();
                byte[] fileBytes = Encoding.UTF8.GetBytes(extractedText);
                return File(fileBytes, "text/plain", "ExtractedText.txt");
            }

            TempData["ErrorMessage"] = "No text available to download.";
            return RedirectToAction("Index");
        }
    }
}
