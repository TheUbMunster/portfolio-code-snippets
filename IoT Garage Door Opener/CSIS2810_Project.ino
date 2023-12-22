#include <dummy.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <WString.h>

IPAddress apIP(192, 168, 4, 1);  // Defining a static IP address for the Server

// This is what the clients will need to connect to:
const char* ssid = "ESP-SERVER";    // you can replace this with any other name
const char* password = "password";  // you can replace this with any other password
String msgBody = "";
String door = "door/";
String light = "light/";

ESP8266WebServer server(80);        // Use the default port 80 for HTTP comms

void setup() {

	pinMode(D4, OUTPUT);
	pinMode(D5, OUTPUT);
	pinMode(D8, OUTPUT); //door
	pinMode(D7, OUTPUT); //light
	Serial.begin(9600);
	Serial.println();
	Serial.println("Configuring access point...");

	//setup the custom IP address
	WiFi.mode(WIFI_AP_STA);

	// COnfigure the Access Point
	WiFi.softAPConfig(apIP, apIP, IPAddress(255, 255, 255, 0));   // subnet FF FF FF 00  

	// Start the Access Point
	WiFi.softAP(ssid, password);
	digitalWrite(D4, LOW);
	// Serial messages with Access Point details
	IPAddress myIP = WiFi.softAPIP();
	Serial.print("AP IP address: ");
	Serial.println(myIP);
	server.on("/message", handleMessage);
	server.begin();
	Serial.println("HTTP server started");
}

void loop() {
	// Constantly listen for Client requests
	server.handleClient();
}

void handleMessage()
{
	if (server.hasArg("body")) {
		digitalWrite(D5, HIGH);
		msgBody = server.arg("body");
		server.send(200, "text/plain", "ack");
		if (msgBody == door)
		{
			digitalWrite(D8, HIGH);
			delay(500);
			digitalWrite(D8, LOW);
		}
		else if (msgBody == light)
		{
			digitalWrite(D7, HIGH);
			delay(500);
			digitalWrite(D7, LOW);
		}
		digitalWrite(D5, LOW);
	}
}