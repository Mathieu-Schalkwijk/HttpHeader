# HttpHeader

## How to launch the server
1. Open Visual Studio
2. Open solution NewWebRunner.sln in folder NewWebRunner
3. Launch "Program.cs"
4. Open a browser and navigate to http://localhost:5000/
5. Launch any scenario

## How requests and scenarios work
The graphical interface is a dynamic web page. By clicking on one of the buttons, a request is sent to the server on port 5000 containing the scenario number to be performed. The server retrieves this number and calls the corresponding class for this scenario, and then returns the result. This result will then be displayed on the web interface.

## Scenarios
### Obtaining the servers used by the websites:
This scenario is useful for identifying which server software is popular among the list of websites. By collecting information about the servers used by various websites, we can extract usefull informations motivated by this use cases:
- Analyzing trends in server software usage
- Assessing security vulnerabilities associated with specific server software
- Selecting the appropriate server software for a project based on popularity or compatibility with existing systems

### Gathering statistics on the age of web pages:
This scenario aims to provide insight into how frequently web pages are updated or modified. The use cases are:
- Identifying outdated or stale content
- Prioritizing web pages for updates or revisions
- Evaluating website maintenance practices and content relevancy

### Counting the number of redirects during the request:
By tracking the number of redirects that occur during a web request, developers can identify potential issues with site navigation, user experience, and search engine optimization (SEO). The use cases are:
- Identifying and resolving site navigation issues
- Enhancing user experience by minimizing redirects
- Improving search engine optimization by reducing unnecessary redirects

### Obtaining response time statistics for web pages:
Monitoring the response times of web pages can help organizations and developers assess site performance, identify potential bottlenecks, and optimize page load times to improve user experience. The use cases are:
- Identifying performance bottlenecks and optimizing page load times
- Improving user experience by ensuring faster page loads
- Enhancing search engine rankings through improved site performance

### Obtaining content type information:
This scenario returns us previsible results because all classic pages are plain text
