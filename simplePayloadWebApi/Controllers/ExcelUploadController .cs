using Microsoft.AspNetCore.Mvc;
using simplePayloadWebApi.Data;
using simplePayloadWebApi.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // for .xlsx files
using Microsoft.AspNetCore.Http;

namespace simplePayloadWebApi.Controllers
{
    [Route("excel")]
    public class ExcelUploadController : Controller
    {
        private readonly simplePayloadContext _context;

        public ExcelUploadController(simplePayloadContext context)
        {
            _context = context;
        }

        [HttpGet("upload")]
        public IActionResult UploadForm() => View();

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            var upload = new CarUpload
            {
                FileName = file.FileName,
                UploadedAt = DateTime.UtcNow,
                Status = "Processing"
            };

            _context.CarUploads.Add(upload);
            await _context.SaveChangesAsync();

            const int batchSize = 250;
            var batch = new List<Car>();

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0);
                int rowCount = sheet.PhysicalNumberOfRows;

                for (int row = 1; row < rowCount; row++) // assuming first row is header
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow == null || currentRow.Cells.All(c => c.CellType == CellType.Blank)) continue;

                    var car = new Car
                    {
                        Brand = currentRow.GetCell(0)?.ToString()?.Trim(),
                        Model = currentRow.GetCell(1)?.ToString()?.Trim(),
                        HorsePower = Convert.ToInt32(currentRow.GetCell(2)?.ToString()),
                        ZeroTo100 = Convert.ToDouble(currentRow.GetCell(3)?.ToString()),
                        CarUploadId = upload.Id
                    };

                    batch.Add(car);

                    if (batch.Count >= batchSize)
                    {
                        await _context.Cars.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                        batch.Clear();
                        _context.ChangeTracker.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    await _context.Cars.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                }

                upload.Status = "Completed";
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                upload.Status = "Failed";
                await _context.SaveChangesAsync();
                return BadRequest("Error while processing file: " + ex.Message);
            }

            return RedirectToAction("UploadSuccess", new { id = upload.Id });
        }

        [HttpGet("success/{id}")]
        public async Task<IActionResult> UploadSuccess(int id)
        {
            var upload = await _context.CarUploads.FindAsync(id);
            return View(upload);
        }
    }
}
