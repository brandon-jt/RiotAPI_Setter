# RiotAPI-Setter
Backend of the RiotAPI Web App.
Connects to an AWS Database and uses the RiotAPI to gather matches of the top 200 players in North America to fetch the item builds and rune setups used on each Champion in the game.
The backend will format this data properly to store in an RDS database, where the Django app can query the data based on champion name to provide a quicker return time. 
<br> Web App: https://github.com/brandon-jt/RiotAPI_WebApp
