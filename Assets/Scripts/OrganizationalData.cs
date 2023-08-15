using UnityEngine;

public class OrganizationalData : MonoBehaviour
{
    public struct Employee
    {
        public string name;
        public int managerIndex;
    }

    public Employee[] employees = new Employee[]
    {
        new Employee { name = "John Doe", managerIndex = -1 },
        new Employee { name = "Jane Doe", managerIndex = 0 },
        new Employee { name = "Jim Smith", managerIndex = 1 },
        new Employee { name = "Sarah Johnson", managerIndex = 2 },
        new Employee { name = "Mike Brown", managerIndex = 1 },
    };

    private void Start()
    {
        // Use the employee data to create a network graph visualizing the org structure
        // ...
    }
}
