# Employee Management Application

This is a Web application developed as part of my academic project during my Master's in Computing at Sheffield Hallam University. This application is built using the **.NET MVC (Model-View-Controller)** framework in C# with Visual Studio.

---

## Table of Contents
- [Application Features And Screenshots](#application-features-and-screenshots)
- [Installation](#installation)
- [Clone the Repository](#clone-the-repository)
- [Steps to Run the Application](#steps-to-run-the-application)

---

## Application Features and Screenshots

### Manager Features

1. **Dashboard**  
   - View number of staff available, ongoing projects, leave balances, and notifications.  
   - Add new staff, projects, and tasks.  

   ![image](https://github.com/user-attachments/assets/d20abd89-9a1d-41b7-9033-1edb1316bf03)

2. **Staff Page**  
   - Manage staff details (view, add, edit).  

   ![image](https://github.com/user-attachments/assets/9ecdbe82-e926-4558-856c-3cc44f35b7e4)
   
   ![image](https://github.com/user-attachments/assets/cbb7a760-a512-4ea3-83d2-b6d0c4903523)

4. **Projects Page**  
   - View, manage, and delete projects.  

   ![image](https://github.com/user-attachments/assets/8f1a1e5a-f460-411b-96a3-4152d0ddecef)
   
   ![image](https://github.com/user-attachments/assets/38ed8727-4355-433d-af97-cc50acefc0bc)

6. **Tasks Page**  
   - View the list of projects.  
   - Add tasks under each project or view existing tasks.  

   ![image](https://github.com/user-attachments/assets/378d620d-198b-4ef7-ae53-40e919247ae4)

4.1. **Add New Task**  
   - Allows the manager to add tasks for their staff.  
   - Restrictions:  
     - Managers cannot assign more than 8 hours per day for a staff member.  
     - Tasks cannot be added on weekends or holidays.  

   ![image](https://github.com/user-attachments/assets/6346a1ed-b71b-4adc-97b9-d3cb43870d8d)  
   
   ![image](https://github.com/user-attachments/assets/5892921b-5bf2-42b0-abd6-2b8d410aaa27)  
   
   ![image](https://github.com/user-attachments/assets/313ee61a-388b-4f19-81df-79b3e6d7fc11)

4.2. **View Tasks**  
   - A task view page with JavaScript-based filters to sort tasks by:  
     - Description.  
     - Start date.  
     - End date.  
     - Staff name.  
   - Managers can:  
     - View reports submitted by staff for the tasks.  
     - Mark tasks as finished.  
     - Edit or delete tasks before they are marked as finished.  

   ![image](https://github.com/user-attachments/assets/df2f2bba-d7eb-45b7-8cb2-40d017dca8e3)  
   
   ![image](https://github.com/user-attachments/assets/95e52a78-b5fb-491a-b366-4b109ae5d32c)

4.2.1. **Task Report**  
   - Managers can view the report submitted by staff after completing a task.  
   - Managers can:  
     - Mark the task as finished.  
     - Reassign the task to the same or a different staff member if it is not completed properly or for other reasons.  

   ![image](https://github.com/user-attachments/assets/681462f4-b510-49df-a272-d3cc035c3b1e)  
   
   ![image](https://github.com/user-attachments/assets/46afaa6f-d0b2-437d-907d-fc43c2bed50b)

5. **Leaves**  
   - Managers can view leave requests from staff.  
   - When a leave request is submitted, managers will see the tasks assigned on the requested leave date along with project deadlines.  

   ![image](https://github.com/user-attachments/assets/967ec3b3-783c-4f35-a325-c15ed4ab168e)

6. **Profile**  
   - View profile.  
   - Change password.  

   ![image](https://github.com/user-attachments/assets/ccca5a20-5ee7-4111-ad52-1b0e895e9a35)

---

### Staff Features

1. **Dashboard**  
   - View pending tasks, leave balance, timesheet, and request leave.  
   - View all tasks.  

   ![staff dashboard](https://github.com/user-attachments/assets/e5cf782a-f880-456e-bafc-8aa2bbbf159b)

2. **View All Tasks**  
   - View tasks.  
   - Submit reports after completing a task.  
   - Edit reports.  

   ![staff dashboard](https://github.com/user-attachments/assets/e5cf782a-f880-456e-bafc-8aa2bbbf159b)
   
   ![View tasks](https://github.com/user-attachments/assets/93adc5bb-9582-41ec-be35-d63ba505f7d0)
    
   ![edit report](https://github.com/user-attachments/assets/3c46fdc0-86c9-4921-afbc-be18e3ba68db)

4. **Timesheet**  
   - View dynamically updated tasks for the current week.

5. **Leaves**  
   - Request leave.  
   - View leave history.  

   ![image](https://github.com/user-attachments/assets/b2c2ae45-f16e-41f7-a774-45ecd7823f0d)

6. **Profile**  
   - View profile.  
   - Change password.  

   ![image](https://github.com/user-attachments/assets/03348abf-e9d1-4410-a96e-f3601a3e0527)

---

### Admin Features

1. **Dashboard**  
   - Admin dashboard (similar to a Director's dashboard) includes:  
     - Shortcut to add new users and new roles.  
     - View remaining leaves.  
     - Approve leave requests submitted by managers.  

   ![image](https://github.com/user-attachments/assets/bd3d2b00-d6bc-4693-998c-371875a7bae9)

2. **Users Page**  
   - View all users, change roles, and remove users.  

   ![image](https://github.com/user-attachments/assets/996b1367-1f2a-447f-9216-0da5a3a2e4dc)

2.1. **Manage User Roles**  
   - Admin can:  
     - Select a user from a dropdown.  
     - Change their role by selecting a new role from the dropdown.  

   ![image](https://github.com/user-attachments/assets/a7ed5bad-a0cb-4803-82af-8940f9531432)

2.2. **Remove User**  
   - Admin can:  
     - Select a user and role from a dropdown.  
     - Remove the selected user.  

   ![image](https://github.com/user-attachments/assets/e49e8e6a-eb71-4e04-9ce4-ce37f798c42a)

---

### Calendar
- The calendar is common across all dashboards.  
- Integrated with **FullCalendar JavaScript API** (https://fullcalendar.io/).  
- Bank holidays are obtained from the **UK Government Bank Holidays API** (https://www.gov.uk/bank-holidays.json).  
- Customizations:  
  - Displays bank holidays and leaves for all users.  
  - Shows project deadlines on the manager dashboard.

---

## Installation

1. [Download Visual Studio](https://visualstudio.microsoft.com/).
2. During installation, select the **ASP.Net and Web Development** workload.
3. Install SQL Server or SQL Server Express for database management.
4. Download SQL Server Management Studio (SSMS) for database administration.

---

## Clone the Repository

Clone the repository using the following command:  
git clone https://github.com/rakshambigai20/EasyTeams.git


---

## Steps to Run the Application

1. Configure the Database
   - Open the appsettings.json file.
   - Update the connection string under the <connectionStrings> section to point to your local SQL Server instance.

2. Run Entity Framework Migrations
   - Navigate to Tools > NuGet Package Manager > Package Manager Console in Visual Studio.
   - Run the following command to create tables and seed data in the database:
     Update-Database

3. Build the Solution
   - In Visual Studio, go to Build > Build Solution or press Ctrl + Shift + B.
   - Ensure the build is successful without errors.

4. Run the Application
   - Press F5 or navigate to Debug > Start Debugging in Visual Studio.
   - The application will launch in your default browser with a URL like:
     http://localhost:7142/

5. Verify Application Functionality
   - Use the following credentials for login:

     - Manager Login:
     Username: mo@mo.com
     Password: Passw0rd!

     - Staff Login:
     Username: rb@rb.com
     Password: Passw0rd!

     - Admin Login:
     Username: admin@admin.com
     Password: Passw0rd!
