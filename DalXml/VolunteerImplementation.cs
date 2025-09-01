namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        if (Read(itemSe => item.Id == itemSe.Id) == null)
        {

            volunteersRootElem.Add(volunteer2XEl(item));
            XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} already exists");
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does Not exist")).Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("ArrayOfVolunteers"), Config.s_volunteers_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        IEnumerable<XElement>  volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements();
        if (filter == null) return volunteers.Select(s => getVolunteer(s));
        else
        {
            return from vol in volunteers
                   let vole = getVolunteer(vol)
                   where filter(vole)
                   select vole;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(s => getVolunteer(s)).FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist")).Remove();

        volunteersRootElem.Add(volunteer2XEl(item));

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// The function turns a Volunteer object to a XElement object
    /// </summary>
    /// <param name="item">The Volunteer object to convert</param>
    /// <returns>The final XElement</returns>
    public XElement volunteer2XEl(Volunteer item)
    {
        return new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("FullName", item.FullName),
            new XElement("PhoneNumber", item.PhoneNumber),
            new XElement("Email", item.Email),
            new XElement("Password", item.Password),
            new XElement("FullAddress", item.FullAddress),
            new XElement("Active", item.Active),
            new XElement("Distance", item.Distance),
            new XElement("VRole", item.VRole),
            new XElement("VDisType", item.VDisType),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude)
            );
    }
    
    /// <summary>
    /// The function converts an XElement to an Volunteer object
    /// </summary>
    /// <param name="s">The XElement to convert</param>
    /// <returns>The final Volunteer object</returns>
    /// <exception cref="FormatException"></exception>
    static Volunteer getVolunteer(XElement s)
    {
        return new DO.Volunteer()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FullName = (string?)s.Element("FullName") ?? "",
            PhoneNumber = (string?)s.Element("PhoneNumber") ?? "",
            Email = (string?)s.Element("Email") ?? "",
            Password = (string?)s.Element("Password") ??"",
            FullAddress = (string?)s.Element("FullAddress") ?? null,
            Active = (bool?)s.Element("Active") ?? false,
            Distance = (double?)s.Element("Distance") ?? null,
            VRole = Enum.TryParse<Role>(s.Element("VRole")?.Value, out var parsedRole) ? parsedRole : Role.Manager,
            VDisType = Enum.TryParse<DistanceType>(s.Element("VDisType")?.Value, out var parsedDisType) ? parsedDisType : DistanceType.Air,
            Latitude = (double?)s.Element("Latitude") ?? null,
            Longitude = (double?)s.Element("Longitude") ?? null
        };
    }

}
