# WCF End-to-End
* Pluralsight's Wcf very well covered course, by Miguel Castro. Complete code.
* Made following the modules, and coded during the course.

## Configurations to have caution.
* If you are using the SQLExpress to store your database, you will need to configure some additional stuff that is not mentioned at the course time, and I think it's better to previously configure, so you'll not have to bother about this in the middle of the way.
And the way to configure the user in SQL Server Express is described in [Add IIS 7 AppPool Identities as SQL Server Logons](http://stackoverflow.com/questions/1933134/add-iis-7-apppool-identities-as-sql-server-logons)

* At least until the time I've wrote this ReadMe, there is nothing talking about how to debug WCF and there was a thing that really helped me to find the answer above.
It talks about hou to crate a behaviour and "bind" it to the service endpoint, and it could be found at the right answer in [Turn on IncludeExceptionDetailInFaults](http://stackoverflow.com/questions/8315633/turn-on-includeexceptiondetailinfaults-either-from-servicebehaviorattribute-or)

## Transport-Level Sessions

* Transport session allow service to identify its clients
  * Kind of like a hot, open connection between Service and Client

* Some bindings support transaction 
  * netTcpBinding
  * netNamedPipeBinding
  * wsHttpBinding - simulate
    * With Reliability or Security turned on - to be explained, stand by