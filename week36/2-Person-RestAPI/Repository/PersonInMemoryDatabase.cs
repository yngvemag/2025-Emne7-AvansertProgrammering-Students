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

    public async Task<Person?> GetByIdAsync(long id)
    {
        await Task.Delay(10);
        return DbStorageList.FirstOrDefault(x => x.Id == id);
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        await Task.Delay(10);
        return DbStorageList;
    }

    public async Task<Person?> UpdateAsync(Person person)
    {
        await Task.Delay(10);
        var p = DbStorageList.FirstOrDefault(p => p.Id == person.Id);
        if (p is null) return null;

        p.FirstName = person.FirstName;
        p.LastName = person.LastName;
        p.Age = person.Age;
        return p;
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