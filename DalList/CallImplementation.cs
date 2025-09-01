namespace Dal;

using DalApi;
using DO;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Call? toDelete = Read(c => c.Id == id);

        if (toDelete == null)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exists");

        DataSource.Calls.Remove(toDelete);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    => DataSource.Calls.FirstOrDefault(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        bool found = false;
        foreach (Call call in DataSource.Calls)
        {
            if (call.Id == item.Id)
            {
                found = true;
                DataSource.Calls.Remove(call);
                DataSource.Calls.Add(item);
            }
        }
        if (!found) throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exists");
    }
}

