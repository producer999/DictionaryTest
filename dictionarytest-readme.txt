Dictionary Test (UWP)
_______________

Description:

The beginning tests of the Dictionary Tools and future projects involving language learning. The purpose of this test is to learn to insert Unicode data into an SQLite database, retrieve it, display it and delete it.

References:

http://bsubramanyamraju.blogspot.in/2016/12/windows-10-uwp-sqlite-how-to-store-data.html
http://www.cfilt.iitb.ac.in/~hdict/webinterface_user/index.php
http://www.c-sharpcorner.com/UploadFile/2b876a/consume-web-service-using-httpclient-to-post-and-get-json-da/


Coming Soon:

****when adding definitions, only refresh the newest one instead of refreshing the entire dictionary (prevent flikr)
****when pressing Enter in def entry fields, activate the Add Contact Button
****do a check on Add Contact that the Word is not in english, devanagari only
****do a test of reading in definitions from a text file and parsing them
****store alternate definitions, alternate forms and examples as JSON in the databse
****if word is changed on the details page, immediately change the google and imported definitions in the textboxes and enable the edit button



v 0.00.02 5/29/2017
-added display of all items in the database on MainPage.loaded()
-clear text fields when new definition is added
-added clear all button functionality to clear the database
-verified that devanagari text can be stored and retrieved from the database
-added DetailsPage.xaml for displaying and updating definitions from the list
-added back button to details page to go back to the main screen
-update databse structure to accomodate more details on each definition
-added functionality to the Update button on the definition details page
-used ListView internal ScrollViewer to scroll elements
	-put ListView inside a Grid instead of StackPanel bc StackPanel breaks ScrollViewer
-update button on details page only enabled if you have changed text in one of the fields
-made some fields IsReadOnly = true on details page
-add google translate functionality via Google Translate/JSON.net
-parse Google API JSON translation response and add it to new Definition constructor using custom classes
-if you add a Word but no Definition, grab the defintion (first one) from Google Translate



v 0.00.01 5/27/2017

-build DBHelper public class with functions for creating manipulating the database
-added building of database to App constructor
-built rough layout for testing inserting definitions into the database and reading them back
-added add item to database functionality