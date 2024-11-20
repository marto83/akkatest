# Akka.Net test project

This project is a simple test to see how many messages per second we can process with Akka.Net

## Key HTTP Routes

* https://localhost:{ASP_NET_PORT}/rewards/generate/{count} - Generate a number of acitivity messages to process
* https://localhost:{ASP_NET_PORT}/rewards/users/{userId} - Get the number of messages processed for a user and any streaks they might have. The only possible userIds are ["123", "124", "125","126","127","128","129","130","131","132","133","134","135","136","137","138","139","140"]

## Testing

Run the application. Post to the generate endpoint with the number of activity messages you want to create and check the console logs how long it took. 

On an M1 Macbook Pro, I was able to process 1,000,000 messages in 1.5224984 seconds which means we can process a message in 0.0015224984 ms and it consumed ~ 350MB of memory. 