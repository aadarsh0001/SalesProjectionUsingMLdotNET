# SalesProjection Using ML.NET/C#/MVVM/WPF/SQL

Welcome to the SalesProjection project repository! This application employs various cutting-edge technologies, including ML.NET, C#, MVVM, Microsoft SQL Server, and WPF, to provide you with the capability to predict sales for future days. By leveraging historical data such as previous actual sales and previous projected sales, this application assists in generating sales projections.

## Technologies Used

- ML.NET
- C#
- MVVM (Model-View-ViewModel) architectural pattern
- Microsoft SQL Server for robust database management
- WPF (Windows Presentation Foundation) for a modern and interactive user interface

## Getting Started

Follow these steps to set up the project in Visual Studio:

### Requirements

- Visual Studio (Any version is compatible)
- Microsoft SQL Server
- .NET 5.0 (preferred)

1. Clone or download the project repository to your local machine.
2. Open Microsoft SQL Server and set up a database. Execute the stored procedure provided in the `ML_POC.sql` file.
3. Unzip or open the downloaded folder and double-click on `HourlySalesReport.sln` to open the project in Visual Studio.
4. Make a configuration adjustment: Search for `settings.settings` in the Solution Explorer's search bar and open `settings.settings`.
5. In the `settings.settings` page, locate the `data source` setting and update it with your server's name as demonstrated in `screenshot-3`. Save the changes and close the `settings.settings` page.
6. Click the "Start" button in Visual Studio to run the application.

Please refer to the following screenshots for guidance:

![Screenshot-1](![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/bbffc772-8648-479b-a73a-c2fe49f544c4))
![Screenshot-2](![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/a05ceaef-84e5-4e92-bd6c-e78f89a428ac)
)
![Screenshot-3](![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/6ab88ef4-7c49-4548-99a3-52cb665e9385)
)

## Team Members

A special acknowledgment to the following team member for their invaluable contribution to this project:

- Chaitanya Ajabe: [GitHub Profile](https://github.com/LinUxTo5re)

## Let's Collaborate

Feel free to explore the codebase, contribute, and take advantage of this application's capabilities in predicting sales projections through ML.NET, C#, MVVM, WPF, and SQL Server. If you encounter any issues, have questions, or would like to collaborate, please don't hesitate to reach out. Together, let's push the boundaries of sales projection using advanced technologies. Happy coding!
