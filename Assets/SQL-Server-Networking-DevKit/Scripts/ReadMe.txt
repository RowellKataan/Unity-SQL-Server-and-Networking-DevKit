Unity SQL Server Toolkit
by      : Michael Marzilli
version : 1.00.0000
released: Apr 20, 2016


ASSET DESCRIPTION
=================
Unity SQL Server Toolkit serves three functions in one easy to use Asset.

Function #1 allows a Unity developer to quickly generate a custom class, and then generate all the code for that class.  
This includes a Base Class, a user-modifiable Class, a Class Editor/Inspector, a Singleton Class Manager, 
a Class Manager Editor/Inspector and the SQL statements necessary to create your tables and stored procedures to 
Insert, Update, Select and Delete your class's Data.

Function #2 allows a Unity developer to interface with a Microsoft SQL Server Database using an easy to use Data Access Layer (DAL).

Function #3 offers the Unity developer a basic, working framework to enable Networking in their application, using Unity's UNET.


REQUIREMENTS
============
In order to use this asset, you will need to be using Unity v5.3 (or higher) and you will have to have access to a Microsoft SQL Server database.
Luckily, Microsoft now allows users to download SQL Server Express for free.
You can find the installer here: https://www.microsoft.com/en-us/server-cloud/products/sql-server-editions/sql-server-express.aspx
You should also download the SQL Server Management Studio, along with your installation of SQL Server.
This will allow you to see and edit your data, as well as create your tables.


INSTALLATION
============
Install Microsoft SQL Server and SQL Server Management Studio  (see above for the link to download for free).
Open SQL Server Management Studio and create a new database called "SQLTEST".
Import this asset into your Unity project.
Double click on the "CreateDemoDatabase" SQL file.  This should load into SQL Server Management Studio.
(if the file does not open in SQL Server Management Studio, you can open the file in a text editor, copy the text [CTRL-A, CTRL-C],
then paste it into a New Query in SQL Server Management Studio).
Inside SQL Server Management Studio, click the "Execute" button.
This script will create the necessary User Login account, and a test table filled with demo data.


USING THE CUSTOM CLASS GENERATOR
================================
According to Microsoft, a class is a construct that enables you to create your own custom types by grouping together 
variables of other types, methods and events. A class is like a blueprint. It defines the data and behavior of a type.

This asset lets you create a class by defining the properties/variables within the class, as well as the basic functions
needed to access and manipulate those properties/variables.  The functions created by this toolkit allows you to search for a specific
dataset, and load and save your data to a database table.

A) OPENING THE CLASS BUILDER

In the main menu toolbar, click "Tools" --> "Class Builder".  This should open the Class Builder window.

Along the left side of the Class Builder window will be a listbox containing a list of all the custom classes that you have created.
Clicking on a class in the list will bring up the information about that class.
If you hold down the Control (CTRL) key, the "?" button will change to an "X" button.  
Clicking the "X" button will delete your class from the database.  You will be asked if you are sure first.
Any class files generated previously will not be deleted.  You will need to remove them from your project manually.

Along the bottom of the Class Builder window will be a total count of the classes you have created, as well as an "ADD NEW" button.
Click the "ADD NEW" button to create a new class.

Along the right side of the Class Builder window will be all the information about your custom class.

B) ADDING A NEW CLASS

Click the "ADD NEW" button at the bottom of the Class Builder window.
In the "Class Name" field, type in the name of your class.  This will be the base name used to name your classes.
Once you have named your class, click the "INITIALIZE CLASS" button to create the structure for your class.
The window will refresh with all the available options to define your class.
If your class belongs to a namespace, type that name into the Namespace field.

C) CLASS CONFIGURATION

There are four (4) parts of the Class Configuration section: Uses Unity, Uses Other Managers, Uses SQL Database and MS-SQL Server Setup.

C.1) USES UNITY

If this class is going to be used within Unity (which, it more likely will be), check this box.
If you plan on using Unity UI elements, such as Toggles, Sliders, Text and the like, check the "Uses Unity UI" box.
If you want this Toolkit to generate a custom inspector/editor for your class, check the "Create Inspector" box.
If you want this Toolkit to generate a Singleton Class manager for your class, check the "Create Class Manager" box.
If you want this Toolkit to generate a custom Serialize/Deserialze function set, check the "Serializer/Deserializer" box.

C.2) USES OTHER MANAGERS (SINGLETONS)

Sometimes developers use their own custom classes to handle the Application, or Networking.  Usually, they do this with an
ApplicationManager or a NetworkManager, in the form of a Singleton.  This Toolkit can automatically create links to the instances
of these specialized managers.

If you use an ApplicationManager, check the "ApplicationManager" box.
If you use a NetworkManager, check the "NetworkManager" box.

C.3) USES SQL DATABASE

If you plan on storing your class data to a SQL Database, this is the section that you need to fill in.

If you want your class to use a SQL database, check the "Uses SQL Database" box.
If you want this Toolkit to generate the code that Loads your class data from a SQL database, check the "Handle Loads" box.
If you want this Toolkit to generate the code that Saves your class data to a SQL database, check the "Handle Saves" box.
If you plan on using the included DatabaseManager prefab in your project, check the "DatabaseManager" box.
If you are not using the DatabaseManager, and you plan on using a SQL database to store your class data, you must define
how your class will connect to the Database.   Leaving DatabaseManager unchecked will bring up the fourth section,
MS-SQL Server Setup.

C.4) MS-SQL SERVER SETUP

Without a DatabaseManager object in your project hierarchy, you will need to instruct your class how to connect to your database.

Provide the server name in the Server field.  This is usually the "computer name\database instance".
For example:  localhost\sqlexpress
Provide the database name in the Database field.  For example: SQLTEST
Provide the username of the login account in the Username field.  For example: SQLUSER
Provide the password of the login account in the Password field.  For example: Password1
If your login user account is associated with a Windows username, check the "Use Windows User Acct" box.
This will hide the Username and Password fields.

D) CREATE NEW CLASS FIELD

This section is where you define the fields (properties/variables) within your class.  Each class is automatically generated with four (4)
standard, mandatory fields: ID, DateCreated, DateUpdated and IsActive.  These cannot be modified or deleted, as they are used by the system.

You may create additional fields.  

Type the name of the field in the Name textbox.  This name cannot already exist in the class, nor can it be the name of an existing enum.

Select the the type of the field from the Field Type drop down list. 
Field types include "int" (integer, whole numbers), "string" (text), "bool" (boolean, true/fale),
"float" (number with decimal places), "enum" (a user defined enumeration, see "Enum Builder" later in this document),
"datetime" (stores a date and time), Vector2 (x/y), Vector3 (x/y/z) and Quaterion (x/y/z/w).

If you select the type "string", you can set the maximum number of characters that can be stored in the field.  Entering "0" into the Max Len
text box denotes that your string field has no maximum limit.

If you select the type "enum", select the user defined enum type from the drop down list.  You can use the Enum Builder to create your own
Enumerations.

If you want your field to have a default value when it is initially created, type that into the "Default Value" text box.

If you want to be able to quickly search on this field, check the "Can Find" box.

When you've completed filling in the fields, click the "ADD" button to add the field to your class.
You should see your new field added to the list at the bottom of the window.

If you ever wish to remove/delete a field, click the "X" button next to the field.  Again, you will not be able to delete the ID,
DateCreated, DateUpdated and IsActive fields.

Once your class has been completed (you have configured the class and added all the necessary fields), click the "SAVE" button at the 
bottom of the window. This will store the class to a Unity database.

Clicking the "CANCEL" button will cancel your changes to the class.  Any work done before the CANCEL button was clicked will be lost.

E) GENERATE CLASS FILES

Once you have saved your class, and you're ready to create the scripts, click the "GENERATE CLASS FILES" button.
This toolkit will create all the necessary files to meet the criteria you set in the Class Configuration section.
The files will be stored in a directory called "Class Scripts", inside its own subdirectory.

In order to complete the creation of your class, you will need to double-click on the SQL Setup File.  The format is 
<class name>_Class_Setup  (for example: Test_Class_Setup).
This will open SQL Server Management Studio.  Click the "Execute" button to run the script to create all the necessary tables and
stored procedures in your database.

F) FILE DESCRIPTIONS

<class name>Base    : This is the base class, and should not be edited.  It contains the base fields, and functions necessary for your
example: TestBase     class to operate.
<class name>        : This is the user modifiable class that you can change to suit the needs of your application.
example: Test         Once created, it will not be overwritten in the future, thus saving your work from being lost.
<class name>Manager : This is the Manager (Singleton) class which allows you to track a list of your classes (such as for a user list).
example: TestManager  If attached to an object in the project hierarchy, and the application is running, it allows you to see and
                      edit all the records in the database for this class.

G) ENUM BUILDER

According to Microsoft, the enum keyword is used to declare an enumeration, a distinct type that consists of a set of named constants 
called the enumerator list.  For example, if you wanted to track the different types of users you were tracking, you could either assign
them a number type, where Normal User = 1, Privileged User = 2 and Admin User = 3.  With an enumeration, you use a pre-defined type.
So, instead of having "UserType = 3", you could have "UserType = UserTypes.Admin".  That way, you don't have to remember what number maps
to what type of user.

The Enum Builder in this toolkit allows you to define an enumeration, so that you can use it in your classes.

In the main menu toolbar, click "Tools" --> "Enum Builder".  This should open the Enum Builder window.

Like the Class Builder window, the Enum Builder window is separated into three (3) parts:  the list of your custom enumerations on the
left, the number of custom enumerations and an "ADD NEW" button on the bottom, and an editor on the right side.

Click the "ADD NEW" button to create a new enumeration.

Type in the name of the enumeration in the Enum Name text box.  You can use your own naming convention. 
Personally, I use the plural of a word.  For example "UserTypes".

Next, type in the member that you want to add to the enumeration in the Member Name text box.  Then click the "ADD" button to add it.

As with the Class Builder window, clicking the "X" button next to an enumeration member will remove it from the enumeration.

Also, holding the Control (CTRL) key and clicking the "X" button next to an enumeration in the left list box will remove the entire
enumeration from the database.  If an enumeration is used in a class in the Class Builder, you will not be able to remove the enumeration
until it is removed from the active classes.

When you are finished adding members to the enumeration, click the "SAVE" button.

Clicking the "CANCEL" button will cancel your changes to the enumeration.  Any work done before the CANCEL button was clicked will be lost.



USING THE DATABASE MANAGER IN YOUR PROJECT
==========================================

The DatabaseManager allows you to define a SQL Server that you want your application to connect to, and accesses calls which can send or receive
data to/from that database.

WARNING: Using the DatabaseManager exposes your SQL Server/Database.  The DatabaseManager is designed to be used with Unity Applications which
will be used internally, such as for a game server, where the user does not have direct access to the application.  Decompiling an application
that uses this DatabaseManager can expose the SQL Server name, as well as the log in credentials to access the server.  The DatabaseManager
should never be used in applications that are distributed to the general public.  It should only be used in applications which will be run 
internally, under your own direct ownership and control.


That being said, the DatabaseManager consists of three main functions: Open a connection to the Database, process data, Close the connection.
Data can be transferred using Stored Procedures, as well as with Direct SQL statement calls.

To begin a SQL transaction, check to see if there is an active connection.  This is done by using the Database.IsConnected call.
For example:

		private void			Start()
		{
			StartCoroutine(DoStart());
		}

		private IEnumerator		DoStart()
		{
			int i = 0;
			Util.Timer clock = new Util.Timer();
			clock.StartTimer();
			while (!Database.IsConnected && clock.GetTime < 4)
			{
				yield return null;
				i++;
			}
			clock.StopTimer();
			Debug.Log("Database Connected = " + Database.IsConnectedCheck.ToString() + " (After " + clock.GetTime + " seconds, " + i.ToString() + ")");
		}



In the above example, the application is checking to see if the connection is open.  In the "IsConnected" property, if the connection is not
open, it will be opened automatically.  This saves time.  The "IsConnectedCheck" property tells you whether or not the database is connected
without forcing a connection to be opened if no connection already exists.

Once the connection is open, you are free to issue commands that send data to the database, or retrieve data from the database.


For example:


		public	void			PressTestButton1()
		{
			// MAKE SURE THE SQL SERVER IS CONNECTED
			// (IF IT ISN'T, A CONNECTION WILL BE OPENED)
			if (Database.DAL.IsConnected)
			{ 
				// QUERY THE DATABASE -- COUNT THE NUMBER OF RECORDS IN OUR TEST TABLE
				Database.DAL.ClearParams();
				int i = Database.DAL.GetSQLSelectInt("SELECT COUNT(*) FROM tblTest");

				// DISPLAY THE RESULTS (OR ERRORS) 
				Debug.Log((Database.DAL.Errors == "") ? "Success!" : "Failed!");
				if (Database.DAL.Errors == "")
					Debug.Log("There are " + i.ToString() + " Records.");
				else
					Debug.Log(Database.DAL.SQLqueries + "\n\n" + Database.DAL.Errors);
			} else
				Debug.Log("Not Connected to the Database.");
		}



In the above example, the Connection is checked to see if it is active (if it is not open, the DAL will open a connection).
Before each query, you will need to issue a ClearParams() call. This clears out any parameters from previous stored procedure calls.
Next, we make the call to SQL using a direct SQL statement which will return an integer, using the GetSQLSelectInt() method.

If an error is encountered, the Errors property will contain the text of the error message.  Otherwise, Errors will be an empty string.
When no error is encountered, an integer is returned.

After each call, the connection is automatically closed.  So there is no need to make a CloseConnection call yourself.  
The DatabaseManager handles that behind the scenes.



To Do List...






DEFINE PROPERTIES/METHODS OF DATABASEMANAGER



DEFINE PROPERTIES/METHODS OF DAL



DEFINE PROPERTIES/METHODS OF APPNETWORKMANAGER