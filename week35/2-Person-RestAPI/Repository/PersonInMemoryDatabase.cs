using PersonRestAPI.Models;
using PersonRestAPI.Repository.Interfaces;

namespace PersonRestAPI.Repository;

public class PersonInMemoryDatabase : IRepository<Person>
{
    private static readonly List<Person> DbStorageList = [];
    
    public async Task<Person?> AddAsync(Person item)
    {
        await Task.Delay(10);
        DbStorageList.Add(item);
        return item;
    }

    public Task<Person?> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        await Task.Delay(10);
        return DbStorageList;
    }

    public Task<Person?> UpdateAsync(Person item)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        await Task.Delay(10);
        var person = DbStorageList.FirstOrDefault(x => x.Id == id);
        if (person is null) return false;
        DbStorageList.Remove(person);
        return true;
    }
}