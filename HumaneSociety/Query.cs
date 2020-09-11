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
                    case "create":
                        AddNewEmployee(employee.FirstName, employee.LastName, employee.UserName, employee.Password, employee.EmployeeNumber, employee.Email);
                        break;
                    case "read":
                        GetEmployees();
                        break;
                    case "update":
                        UpdateEmployee(employee);
                        break;
                    case "delete":
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

            db.Employees.InsertOnSubmit(newEmployee);
            db.SubmitChanges();
        }

        internal static List<Employee> GetEmployees()
        {//read
            List<Employee> allEmployees = db.Employees.ToList();

            return allEmployees;
        }

        internal static Employee RetrieveEmployeeFirstNameAndLastName(string firstName, string lastName)
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

            var removeEmployee =
                from m in db.Employees
                where m.EmployeeId == employee.EmployeeId
                select m;

            foreach (var item in removeEmployee)
            {
                db.Employees.DeleteOnSubmit(item);
            }
      
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();

        }

        internal static List<Animal> GetAnimals()
        {
            List<Animal> allAnimals = db.Animals.ToList();

            return allAnimals;
        }

        internal static Animal GetAnimalByID(int id)
        {

            Animal animal = db.Animals.Where(a => a.AnimalId == id).Single();

            return animal;
        }
        

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {

            foreach (KeyValuePair<int, string> valuePair in updates)
            {
                Animal animal = db.Animals.Where(a => a.AnimalId == animalId).SingleOrDefault();

                //Switch case here 
                switch (valuePair.Key)
                {
                    case 1:
                        //logic in here 
                        break;
                    case 2:
                        
                        break;
                    case 3:
                       
                        break;
                    case 4:
                        
                        break;
                    case 5:
                        
                        break;
                    case 6:
                        
                        break;
                    case 7:
                       
                        break;
                    case 8:
                        
                        break;
                    default:
                        
                        break;
                }
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            var removeAnimal =
                from m in db.Animals
                where m.AnimalId == animal.AnimalId
                select m;

            foreach (var item in removeAnimal)
            {
                db.Animals.DeleteOnSubmit(item);
            }
            
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static List<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {


            //trying stuff out
            List<Animal> animalsThatFitCriterion = null;
            List<Animal> animalSearched = GetAnimals();

            List<Animal> animals = GetAnimals();
            foreach (KeyValuePair<int, string> valuePair in updates)
            {

                if (valuePair.Key == 1)
                {
                     animalsThatFitCriterion = animals.SkipWhile(a => a.CategoryId.ToString() != valuePair.Value).ToList();
                }

                else if (valuePair.Key == 2)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.Name != valuePair.Value).ToList();
                }

                else if (valuePair.Key == 3)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.Age.ToString() != valuePair.Value).ToList();
                }

                else if (valuePair.Key == 4)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.Demeanor != valuePair.Value).ToList();
                }
                else if (valuePair.Key == 5)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.KidFriendly.ToString() != valuePair.Value).ToList();
                }

                else if (valuePair.Key == 6)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.PetFriendly.ToString() != valuePair.Value).ToList();
                }

                else if (valuePair.Key == 7)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.Weight.ToString() != valuePair.Value).ToList();
                }

                else if (valuePair.Key == 8)
                {
                    animalsThatFitCriterion = animals.SkipWhile(a => a.AnimalId.ToString() != valuePair.Value).ToList();
                }

                //I first added the animal searched with each animal and with each iteration it should remove all that does not apply anymore
                animalSearched = animalSearched.SkipWhile(i => !animalsThatFitCriterion.Contains(i)).ToList();   
                
            }
           

            return animalSearched;

            
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category category = db.Categories.Where(c => c.Name == categoryName).Single();
            int categoryId = category.CategoryId;
            return categoryId;
            
        }

        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r => r.AnimalId == animalId).SingleOrDefault();
            return room;
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).Single();
            int dietPlanId = dietPlan.DietPlanId;
            return dietPlanId;

        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {

            //combine client and animal
            //use animal and client passed in to create new table


            //ATTRIBUTES OF ADOPT:
            //ClientId 
            //AnimalId 
            //ApprovalStatus
            //AdoptionFee (75)
            //PaymentCollected

            Adoption adoption = new Adoption();

            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            //after mvp comeback and fix hardcode
            
            if (UserInterface.GetBitData($"The adoption cost is {adoption.AdoptionFee}! Would you like to pay right now? \n" +
                $"Type yes or no: "))
            {
                adoption.PaymentCollected = true;
            }
            else
            {
                adoption.PaymentCollected = false;
            }

            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            //after mvp come back and fix hardcode
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending"); 

            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted)
            {
                adoption.ApprovalStatus = "Adopted";
                if (!(bool)adoption.PaymentCollected)
                {
                    UserInterface.DisplayUserOptions($"Your adoption was approved! The adoption cost is {adoption.AdoptionFee}!");
                    adoption.PaymentCollected = true;
                }
                else
                {
                    UserInterface.DisplayUserOptions("Your adoption was approved! Have a nice day!");
                }
            }
            else if (!isAdopted)
            {
                adoption.ApprovalStatus = "Adoption Declined";
                if ((bool)adoption.PaymentCollected)
                {
                    UserInterface.DisplayUserOptions("Your adoption was declined! Your adoption fee will be returned to you!");
                    adoption.PaymentCollected = false;
                }
                else
                {
                    UserInterface.DisplayUserOptions("Your adoption was declined! Sorry have a nice day!");
                }
            }

            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var removeAdoption =
                from a in db.Adoptions
                where a.AnimalId == animalId && a.ClientId == clientId
                select a;

            foreach (var item in removeAdoption)
            {
                db.Adoptions.DeleteOnSubmit(item);
            }

            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var animalsShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
        
            return animalsShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            //Shot table attributes:
            //ShotId
            //Name

            //AnimalShot table attributes:
            //AnimalId
            //ShotId
            //DateRecieved
            AnimalShot animalShot = new AnimalShot();
            Shot shot = db.Shots.Where(s => s.Name == shotName).First();
            animalShot.AnimalId = animal.AnimalId;
            animalShot.DateReceived = DateTime.Now;
            db.SubmitChanges();
        }
    }
}