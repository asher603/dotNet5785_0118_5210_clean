namespace Dal;

using DO;
using DalApi;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(itemSe=>item.Id==itemSe.Id) != null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        
        DataSource.Volunteers.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer? toDelete = Read(v => v.Id == id);

        if (toDelete == null) 
            throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exists");

        DataSource.Volunteers.Remove(toDelete);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    => DataSource.Volunteers.FirstOrDefault(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
{
        bool found = false;
        foreach (Volunteer v in DataSource.Volunteers)
        {
            if (v.Id == item.Id)
            {
                found = true;
                DataSource.Volunteers.Remove(v);
                DataSource.Volunteers.Add(item);
            }
        }
        if (!found) throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exists");
    }
}
