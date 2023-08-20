using Microsoft.EntityFrameworkCore;

namespace PetStore.Services
{
    public class PetStoreDbService
    {
        private readonly PetStoreContext _context;

        public PetStoreDbService(string connection)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<PetStoreContext>();
            dbContextBuilder.UseSqlServer(connection);
            _context = new PetStoreContext(dbContextBuilder.Options);                
        }

        public async Task<List<Pet>?> GetPets()
        {
            // Ensure Pets dbSet exists
            if (_context.Pets == null) return null;

            return await _context
                .Pets                
                .ToListAsync();
        }

        public async Task<Pet?> GetPetById(int? id)
        {
            // Ensure Pets dbSet exists
            if (_context.Pets == null) return null;

            // If no id passed in then return null
            if (!id.HasValue) return null;

            return await _context.Pets.FirstOrDefaultAsync(m => m.ID == id.Value);                
        }

        public async Task AddPet(Pet pet)
        {
            // Todo: Check if successful and return bool?
            _context.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePet(Pet pet)
        {
            try
            {
                _context.Update(pet);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(pet.ID))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task DeletePet(Pet pet)
        {
            if (_context.Pets != null) _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }

        private bool PetExists(int id)
        {
            return (_context.Pets?.Any(e => e.ID == id)).GetValueOrDefault();
        }

    }
}