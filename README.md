# MARINEYE v0.1

## Documentation
Documentation is attached in documentation/DOKUMENTACJA_TECHNICZNA_MARINEYE_v0.1.pdf file.

## Setup
    Download the repository using the command:
    git clone https://github.com/piotrku91/MARINEYE.git

    Make sure the following are installed on your computer:
        o .NET Runtime 8.0,
        o In the NuGet Package Manager (Top-level packages):
            ▪ Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore (8.0.11)
            ▪ Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.11)
            ▪ Microsoft.AspNetCore.Identity.UI (8.0.11)
            ▪ Microsoft.EntityFrameworkCore.Sqlite (8.0.11)
            ▪ Microsoft.EntityFrameworkCore.SqlServer (8.0.11)
            ▪ Microsoft.EntityFrameworkCore.Tools (8.0.11)
            ▪ Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.7)

    • Go to the downloaded MARINEYE folder and open the solution (MARINEYE.sln) in Visual Studio

    • In the NuGet Package Manager Console, run the command:
    Update-Database -Migration ClearInitial

    • Clean the solution and run it in Release mode

    • After launching, the home page should appear along with a welcome message, and a navigation bar should be available with options to view the fleet, register, and log in to the system
    
    • Register the first account (The first account in the system becomes the main administrator; each subsequent account receives the lowest level of permissions)
    
    • After registration and login, the system is ready for further use
