# TextExtraction

**TextExtraction** is a project aimed at developing a system for automatic text recognition from images using OpenCV and Tesseract OCR libraries. The project is being developed as part of a graduation thesis.

---

## Features

- **Upload images** and process them to enhance OCR quality.
- **Recognize text** from images using Tesseract OCR.

---

## Technologies Used

- **Programming Language**: C#
- **Libraries**: OpenCV, Tesseract OCR
- **Framework**: ASP.NET Core
- **Database**: SQL Server

---

## Installation

### Requirements

- .NET SDK (version 6.0 or higher)
- SQL Server
- Installed Tesseract OCR package
- Visual Studio or another IDE for development

### Installation Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/chicha888/TextExtraction.git
   ```
2. **Navigate to the project directory:**
   ```bash
   cd TextExtraction
   ```
3. **Install dependencies** via NuGet Package Manager.
4. **Configure the database connection** in the `appsettings.json` file:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;"
     }
   }
   ```
5. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```
6. **Run the application:**
   ```bash
   dotnet run
   ```

---

## Usage

1. **Open a browser** and navigate to `http://localhost:5000` (or another port specified in the settings).
2. **Register or log in** to the system.
3. **Upload an image** and start the text recognition process.
4. **Download the result** or view it in the history section.

---

## Project Structure

- `Controllers` - contains logic for handling HTTP requests.
- `Models` - defines the main entities used in the project.
- `Views` - responsible for the user interface.

---

## Development Plans

- **Add support** for processing PDF documents.
- **Implement an API** for integration with other services.
- **Improve algorithms** for image preprocessing.
