# Guest and Sales Projection Using Machine Learning in C# (ML.NET)

Welcome to the Sales and Guest Projection project repository!

- Explore the Sales and Guest Projection repository, utilizing ML.NET, C#, MVVM, Microsoft SQL Server and WPF.
- Predict future guests using advanced tech - historical data fuels projections for upcoming days.
- Tap into past actual and projected guests for accurate guest forecasts, a powerful business tool.
- The same logic can be applied for future sales projections using machine learning with the help of ML.NET.
<br>
     
## Some Best projections
  **Projection - 1:** <br>
  ![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/99c01e95-2589-4327-a251-d14faf9cf387)
  <br> <br>
  **Projection - 2:** <br>
  ![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/183f597d-77d8-4a69-b6f6-b2a804b567f7)
    <br>
## Columns Definition
- **Time** : Column used to describe the timestamp for guest count.
- **Guest Count** : Actual guest count for the specified timestamp.
- **Projected MyStore** : Projected guest count using the Average algorithm in the existing application.
- **Projected ML** : Projected guest count using machine learning (ML.NET).
- **Difference ML** : **Guest Count - Projected ML**
- **Difference SDM** : **Guest Count - Projected MyStore**

## Technologies Used

- ML.NET
- C#
- MVVM (Model-View-ViewModel) architectural pattern
- Microsoft SQL Server for robust database management
- WPF (Windows Presentation Foundation) for a modern and interactive user interface

## Getting Started

Follow these steps to set up the project in Visual Studio:

### Requirements

- Visual Studio (Any version is compatible)     <br>

- Microsoft SQL Server     <br>

- .NET 5.0 (preferred)     <br>


1. Clone or download the project repository to your local machine.
<pre> <code> 
git clone https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET.git 
</code> </pre> 
2. Open Microsoft SQL Server and set up a database. Execute the stored procedure provided in the `ML_POC.sql` file.
3. Unzip or open the downloaded folder and double-click on `HourlySalesReport.sln` to open the project in Visual Studio.
4. Make a configuration adjustment: Search for `settings.settings` in the Solution Explorer's search bar and open `settings.settings`.
5. In the `settings.settings` page, locate the `data source` setting and update it with your server's name as demonstrated in `screenshot-3`. Save the changes and close the `settings.settings` page.
6. Click the "Start" button in Visual Studio to run the application.
<br> <br>
## Please refer to the following screenshots for guidance:
<br> <br>
    **Screenshot-1:** <br>
    ![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/bbffc772-8648-479b-a73a-c2fe49f544c4)
    <br> <br>
    **Screenshot-2:** <br>
    ![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/a05ceaef-84e5-4e92-bd6c-e78f89a428ac)
    <br> <br>
    **Screenshot-3:**<br>
    ![image](https://github.com/aadarsh0001/SalesProjectionUsingMLdotNET/assets/117271222/6ab88ef4-7c49-4548-99a3-52cb665e9385)
    <br> <br>


## Team Members

A special acknowledgment to the following team member for their invaluable contribution to this project:

- Chaitanya Ajabe: [GitHub Profile](https://github.com/LinUxTo5re)

## Let's Collaborate

Feel free to explore the codebase, contribute, and take advantage of this application's capabilities in predicting sales projections through ML.NET, C#, MVVM, WPF, and SQL Server. If you encounter any issues, have questions, or would like to collaborate, please don't hesitate to reach out. Together, let's push the boundaries of sales projection using advanced technologies. Happy coding!
