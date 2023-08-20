using Microsoft.AspNetCore.Mvc;
using PagedList;
using PetStore.Services;
using Serilog;
using System.Text.Json;

namespace PetStore.Web.Controllers
{
    public class PetsController : Controller
    {
        private readonly PetStoreDbService _dbService;

        public PetsController(PetStoreDbService dbService)
        {
            _dbService = dbService;
        }

        // GET: Pets
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, string typeFilter, bool? saved)
        {
            // Ensure we have a db service.
            if (_dbService == null) return Problem("Database Service is null.");

            // Get the pets from db.
            List<Pet>? pets = await _dbService.GetPets();

            // Display not found if nothing returned
            if (pets == null) return NotFound();

            ViewBag.TypeFilter = typeFilter;
            ViewBag.Saved = saved;

            // Set view props
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DobSortParm = sortOrder == "dob" ? "dob_desc" : "dob";
            ViewBag.WeightSortParm = sortOrder == "weight" ? "weight_desc" : "weight";



            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            // Apply type filtering
            if (!string.IsNullOrEmpty(typeFilter))
            {
                pets = pets
                   .Where(p => p.Type == Enum.Parse<PetType>(typeFilter))
                   .ToList();
            }

            // Apply search filtering (case sesitive)
            if (!string.IsNullOrEmpty(searchString))
            {
                pets = pets
                    .Where(p => p.Name.Contains(searchString))
                    .ToList();
            }

            // Apply ordering
            pets = sortOrder switch
            {
                "name_desc" => pets.OrderByDescending(x => x.Name).ToList(),
                "dob" => pets.OrderBy(x => x.DateOfBirth).ToList(),
                "dob_desc" => pets.OrderByDescending(x => x.DateOfBirth).ToList(),
                "weight" => pets.OrderBy(x => x.Weight).ToList(),
                "weight_desc" => pets.OrderByDescending(x => x.Weight).ToList(),
                _ => pets.OrderBy(x => x.Name).ToList(),
            };

            // Pagination
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(pets.ToPagedList(pageNumber, pageSize));
        }                

        // GET: Pets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Type,DateOfBirth,Weight")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                // Ensure we have a db service.
                if (_dbService == null) return Problem("Database Service is null.");

                await _dbService.AddPet(pet);
                                
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        // GET: Pets/Edit/{}
        public async Task<IActionResult> Edit(int? id)
        {
            // Ensure we have a db service.
            if (_dbService == null) return Problem("Database Service is null.");

            // Get the pet to edit
            Pet? pet = await _dbService.GetPetById(id);

            // Display pet or not found if nothing returned
            return pet != null ?
                View(pet) :
                NotFound();
        }

        // POST: Pets/Edit/{}
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Type,DateOfBirth,Weight")] Pet pet)
        {
            // Ensure we have a db service.
            if (_dbService == null) return Problem("Database Service is null.");

            // Ensure updating right pet
            if (id != pet.ID) return Problem("Edit Pet mismatch.");

            if (ModelState.IsValid)
            {
                if (await _dbService.UpdatePet(pet))
                {
                    Log.Information($"Record edited: {JsonSerializer.Serialize<Pet>(pet)}");
                    return RedirectToAction(nameof(Index), new {Saved = "true"});
                }
                else
                {
                    return Problem("Unable to edit Pet.");
                }   
            }

            return View(pet);
        }
                

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Ensure we have a db service.
            if (_dbService == null) return Problem("Database Service is null.");

            // Get the pet to delete
            Pet? pet = await _dbService.GetPetById(id);

            if (pet == null)
            {
                return Problem("Unable to find pet to delete.");
            }
            else
            {
                await _dbService.DeletePet(pet);
                Log.Information($"Record deleted with ID: {id}");

                return RedirectToAction(nameof(Index));
            }
        }               
    }
}
