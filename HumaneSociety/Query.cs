using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        //public void GetEmployeeOperation()
        //{
        //    List<string> options = new List<string>() { "What would you like to do? (select number of choice)", "1. Add animal", "2. Remove Anmial", "3. Check Animal Status", "4. Approve Adoption" };
        //    UserInterface.DisplayUserOptions(options);
            
            
        //}

        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {//take crud operation passed in and apply it to the customer passed in

            //Console.WriteLine("Select the number of the employee action.", "1. Add Employee Record", "2. Get Employee Record", "3. Update Employee Record", "4. Delete Employee Record");
            //int action = int.Parse(Console.ReadLine());
            
                switch (crudOperation)
                {
                    case "Add":
                        AddNewEmployee(employee.FirstName, employee.LastName, employee.UserName, employee.Password, employee.EmployeeNumber, employee.Email);
                        break;
                    case "Get":
                        GetEmployees();
                        break;
                    case "Update":
                        UpdateEmployee(employee);
                        break;
                    case "Delete":
                        RemoveEmployee(employee);
                        break;
                    default:
                        UserInterface.DisplayUserOptions("Input not accepted please try again");
                        RunEmployeeQueries(employee, crudOperation);
                        break;
                }
           

            throw new NotImplementedException();
        }


        internal static void AddNewEmployee(string firstName, string lastName, string username, string password, int? employeeNumber, string email)
        {//Create
            Employee newEmployee = new Employee();

            newEmployee.FirstName = firstName;
            newEmployee.LastName = lastName;
            newEmployee.UserName = username;
            newEmployee.Password = password;
            newEmployee.EmployeeNumber = employeeNumber;
            newEmployee.Email = email;
        }

        internal static List<Employee> GetEmployees()
        {//read
            List<Employee> allEmployees = db.Employees.ToList();

            return allEmployees;
        }

        internal static Employee RetrieveEmployeeFNameAndLName(string firstName, string lastName)
        {//Ignore
            Employee employeeFromDb = db.Employees.Where(e => e.FirstName == firstName && e.LastName == lastName).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }


        internal static void UpdateEmployee(Employee employeeWithUpdates)
        {//Update
            // find corresponding Employee from Db
            Employee employeeFromDb = null;

            try
            {
                employeeFromDb = db.Employees.Where(e => e.EmployeeId == employeeWithUpdates.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a EmployeeId that matches the Employee passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            employeeFromDb.FirstName = employeeWithUpdates.FirstName;
            employeeFromDb.LastName = employeeWithUpdates.LastName;
            employeeFromDb.UserName = employeeWithUpdates.UserName;
            employeeFromDb.Password = employeeWithUpdates.Password;
            employeeFromDb.EmployeeNumber = employeeWithUpdates.EmployeeNumber;
            employeeFromDb.Email = employeeWithUpdates.Email;

            // submit changes
            db.SubmitChanges();
        }

        internal static void RemoveEmployee(Employee employee)

        {

            var RemoveEmployee =
                from m in db.Employees
                where m.EmployeeId == employee.EmployeeId
                select m;

            foreach (var item in RemoveEmployee)
            {
                db.Employees.DeleteOnSubmit(item);
            }
                
            

            db.SubmitChanges();
        }



        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int id)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            throw new NotImplementedException();
        }

        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}