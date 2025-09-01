namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        int id = Config.NextAssingmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Assignment? toDelete = Read(asm => asm.Id == id);

        if (toDelete == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exists");

        DataSource.Assignments.Remove(toDelete);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    => DataSource.Assignments.FirstOrDefault(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
       bool found = false;
       foreach(Assignment asm in DataSource.Assignments)
       {
            if(asm.Id == item.Id)
            {
                found = true;
                DataSource.Assignments.Remove(asm);
                DataSource.Assignments.Add(item);
            }
       }
       if (!found) throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exists");
    }
}
