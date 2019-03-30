Hello There :) 

Please use the following steps to run the Program

1.Open Developer Command Promt for VS 2017(This is what I used to compile mine)

2.Navigate to the project folder so that your current directory is(....\WebCrawler-Patrick Swett)

3A.You can now compile in 2 different ways, If you are using the command prompt I mentioned above it should
come with nmake installed. Typing (nmake build) without the parenthesis will compile Program.cs and produce Program.exe
3B If for some reason typing nmake build does not work you can also copy and paste the below line to 
produce the same result as nmake build
csc /r:requiredDLLs/System.Net.Http.dll /r:requiredDLLs/System.Text.RegularExpressions.dll /r:requiredDLLs/System.Collections.dll /r:requiredDLLs/System.Net.dll Program.cs

4. Now you can run the exe from the command promt by typing Program.exe (arg:website) (arg:hopnumber)
as shown above the first argument should be a website and the second should be the amount of hops
example: Program.exe https://www.google.com/ 5
If you do not enter the arguments as shown above you will recieve messages as to what went wrong such as too few arguments
or invalid website entered.

Additional Notes: You should have .Net version 4.5 or higher installed to use the .net.http library


