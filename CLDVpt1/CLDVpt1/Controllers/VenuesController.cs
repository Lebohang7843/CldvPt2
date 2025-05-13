using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CLDVpt1.Models;
using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace CLDVpt1.Controllers
{
    public class VenuesController : Controller
    {
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=cldv;AccountKey=ZFh2xsl7WPj02lKiEuv1n3IrQixNQMpOphn0NNk62DlErBmeu1DxElB9yq+KB7fz1VvSGh18VSGq+ASt73ODoQ==;EndpointSuffix=core.windows.net";
            var containerName = "cldvpt1";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Venues venue)
        {
            if (ModelState.IsValid)
            {
                if(venue.ImageFile != null)
                {
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                    venue.ImageURL = blobUrl;
                }
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }



        // This method will return a form to delete a venue
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var venue = await _context.Venues
                .FirstOrDefaultAsync(v => v.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // This method will handle the deletion of the venue
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Venue/Edit/{id}
        public async Task<IActionResult> Edit(int? id, Venues venue)
        {
            if(id != venue.VenueID) return NotFound();
            
            if(ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                        venue.ImageURL = blobUrl;
                    }
                    else
                    {

                    }
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(venue);
            }
            if (id == null)
            {
                return NotFound();
            }

            var venueFromDb = await _context.Venues.FindAsync(id);

            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venue/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venues venue)
        {
            if (id != venue.VenueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }



        // This method will return the details of a venue
        // '?' means that the parameter is nullable, which means it can be null or have a value of type int.

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();  // it will return a 404 Not Found error if the id is null
            }

            //This will  find the venue with the specified id and include the events associated with the venue
            var venue = await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.VenueID == id);

            if (venue == null)
            {
                return NotFound();  // it will return a 404 Not Found error if the venue is null
            }

            return View(venue);
        }

        // Check if the venue exists in the database
        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueID == id);
        }
    }

}

