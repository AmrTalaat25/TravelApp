using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace TravelApp.Models.Services
{
    public class AdvertisementImageService : IAdvertisementImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdvertisementImageService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Upload an image to the server
        public async Task<bool> AddImage(AdvertisementImageDto model)
        {
            try
            {
                if (model.Image == null || model.AdID <= 0 || !model.Image.ContentType.StartsWith("image/"))
                {
                    return false;
                }

                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                var extension = Path.GetExtension(model.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return false;
                }

                var image = new AdvertisementImage
                {
                    AdID = model.AdID,
                    Imagepath = Path.Combine("images", model.Image.FileName)
                };

                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.Imagepath);

                // Compress the uploaded image based on the extension
                byte[] compressedImageBytes;

                using (var memoryStream = new MemoryStream())
                {
                    await model.Image.CopyToAsync(memoryStream);
                    compressedImageBytes = CompressImage(memoryStream.ToArray(), extension, -50);
                }

                // Write the compressed image to the path
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    fileStream.Write(compressedImageBytes, 0, compressedImageBytes.Length);
                }

                // Add to database
                _context.AdvertisementImages.Add(image);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] CompressImage(byte[] imageBytes, string extension, long quality)
        {
            using (var memoryStream = new MemoryStream(imageBytes))
            using (var bitmap = new Bitmap(memoryStream))
            {
                // Determine the image format based on extension
                ImageFormat format = extension switch
                {
                    ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                    ".png" => ImageFormat.Png,
                    _ => throw new NotSupportedException("Unsupported image format")
                };

                // Get the encoder for the specified image format
                ImageCodecInfo encoder = GetEncoder(format);
                EncoderParameters encoderParameters = new EncoderParameters(1);


                // Set encoder parameters based on the image format
                if (format == ImageFormat.Jpeg)
                {
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                }
                else if (format == ImageFormat.Png)
                {
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                    // encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, EncoderValue.CompressionLSE);
                }

                using (var outputStream = new MemoryStream())
                {
                    bitmap.Save(outputStream, encoder, encoderParameters);
                    return outputStream.ToArray(); // Return the compressed data as a byte array
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
        #endregion

        #region Get serveral images as a list of base64 strings using advertisement ID
        public async Task<List<string>> GetImagesByAdID(int adID)
        {
            try
            {
                var advertisement = await _context.Advertisements
                    .Include(a => a.Images)
                    .FirstOrDefaultAsync(a => a.AdID == adID);

                if (advertisement == null)
                {
                    return null!;
                }

                var imageBase64Urls = new List<string>();
                foreach (var image in advertisement.Images)
                {
                    var imagePath = image.Imagepath;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        var imagePathOnDisk = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                        if (System.IO.File.Exists(imagePathOnDisk))
                        {
                            var imageBytes = System.IO.File.ReadAllBytes(imagePathOnDisk);
                            var base64String = Convert.ToBase64String(imageBytes);
                            var base64Url = $"data:image/*;base64,{base64String}";

                            imageBase64Urls.Add(base64Url);
                        }
                        else
                        {
                            imageBase64Urls.Add("Couldn't load image");
                        }
                    }
                }

                return imageBase64Urls;
            }
            catch (Exception ex)
            {
                return new List<string>
                {
                    ex.Message
                };
            }
        }
        #endregion

        #region Get a single image in base64 format using image ID
        public async Task<string> GetImageBase64ById(int imageId)
        {
            try
            {
                var image = await _context.AdvertisementImages.FindAsync(imageId);

                if (image == null)
                {
                    return null;
                }

                var imagePath = image.Imagepath;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var imagePathOnDisk = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                    if (System.IO.File.Exists(imagePathOnDisk))
                    {
                        var imageBytes = File.ReadAllBytes(imagePathOnDisk);
                        var base64String = Convert.ToBase64String(imageBytes);
                        var base64Url = $"data:image/*;base64,{base64String}";

                        return base64Url;
                    }
                    else
                    {
                        // TODO Handle Exception
                        return null;
                    }
                }

                return null;
            }
            catch
            {
                // TODO Handle Exception
                return null;
            }
        }
        #endregion

        #region Delete image by image id
        public async Task<string> DeleteImageById(int imageId)
        {
            try
            {
                var image = await _context.AdvertisementImages.FindAsync(imageId);

                if (image == null)
                {
                    return $"Image with Id ${imageId} Not Found";
                }

                var imagePath = image.Imagepath;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var imagePathOnDisk = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                    if (System.IO.File.Exists(imagePathOnDisk))
                    {
                        System.IO.File.Delete(imagePathOnDisk);
                    }
                    else
                    {
                        return $"ImageId {imageId} exists in database but file Not Found";
                    }
                }

                _context.AdvertisementImages.Remove(image);
                await _context.SaveChangesAsync();

                return $"Delted Image with Id {imageId} Successfully";
            }
            catch (Exception ex)
            {
                return $"Exception raised when attempting to delete Image with Id {imageId}: {ex.Message}";
            }
        }
        #endregion

        #region Delete advertisement images
        public async Task<ResponseModel<List<string>>> DeleteAdvertisementImages(int AdId)
        {
            var imageIds = await _context.AdvertisementImages
            .Where(img => img.AdID == AdId)
            .Select(img => img.ImageID)
            .ToListAsync();
            var results = new List<string>();
            var response = new ResponseModel<List<string>>();
            if (imageIds == null)
            {
                return new ResponseModel<List<string>>()
                {
                    Message = $"Could not find advertisement with ID {AdId}",
                    Success = false,
                };
            }
            foreach (var imageId in imageIds)
            {
                try
                {
                    var result = await DeleteImageById(imageId);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Errors!.Add(ex.Message);
                }
            }
            return new ResponseModel<List<string>>
            {
                Data = results,
                Message = "Image deletion complete"
            };
        }
        #endregion

        #region Update single image
        public async Task<bool> UpdateImage(UpdateImageDto updateImageDto)
        {
            try
            {
                var image = await _context.AdvertisementImages.FindAsync(updateImageDto.ImageId);

                if (image == null)
                {
                    throw new ArgumentException($"Image with ID {updateImageDto.ImageId} not found.");
                }

                var imagePath = image.Imagepath;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var imagePathOnDisk = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                    if (System.IO.File.Exists(imagePathOnDisk))
                    {
                        System.IO.File.Delete(imagePathOnDisk);
                    }
                    else
                    {
                        throw new FileNotFoundException($"Image file not found at path: {imagePathOnDisk}");
                    }
                }

                var newImagePath = $"images/{updateImageDto.NewImage.FileName}";
                var newImagePathOnDisk = Path.Combine(_webHostEnvironment.WebRootPath, newImagePath);

                using (var stream = new FileStream(newImagePathOnDisk, FileMode.Create))
                {
                    await updateImageDto.NewImage.CopyToAsync(stream);
                }

                image.Imagepath = newImagePath;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Failed to update image: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Failed to update image: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update image: {ex.Message}");
            }
        }
        #endregion
    }
}
