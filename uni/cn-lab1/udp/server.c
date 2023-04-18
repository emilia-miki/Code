#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <stdio.h>
#include <stdbool.h>

#define PORT 8080
#define N 2
#define M 4

// driver code
int main() {
	int sockfd;
    bool bools[N];
    int ints[M];
	char buffer[N * sizeof(bool) + M * sizeof(int)];
	struct sockaddr_in servaddr;
    struct sockaddr_in cliaddr;
	
	// creating socket file descriptor
	if ((sockfd = socket(AF_INET, SOCK_DGRAM, 0)) < 0) {
		printf("Socket creation failed.\n");
		exit(EXIT_FAILURE);
	}
	
	memset(&servaddr, 0, sizeof(servaddr));
	memset(&cliaddr, 0, sizeof(cliaddr));
	
	// filling server information
	servaddr.sin_family = AF_INET; // IPv4
	servaddr.sin_addr.s_addr = INADDR_ANY;
	servaddr.sin_port = htons(PORT);
	
	// bind the socket with the server address
	if (bind(sockfd, (const struct sockaddr *)&servaddr, sizeof(servaddr)) < 0)
	{
		printf("Bind failed.\n");
		exit(EXIT_FAILURE);
	}
	
	socklen_t len;

	len = sizeof(cliaddr); //len is value/result

    // get data
	recvfrom(sockfd, buffer, N * sizeof(bool) + M * sizeof(int),
				MSG_WAITALL, (struct sockaddr *) &cliaddr,
				&len);
    
    // move data to type-specific arrays
    for (int i = 0; i < N; i++) 
    {
        memcpy(&bools[i], buffer + i * sizeof(bool), sizeof(bool));
    }

    for (int i = 0; i < M; i++) 
    {
        memcpy(&ints[i], buffer + N * sizeof(bool) + i * sizeof(int), sizeof(int));
    }

	// display data
	printf("Booleans: ");
	for (int i = 0; i < N; i++)
	{
		printf(bools[i] ? "true " : "false");
	}
	printf("\n");

	printf("Integers: ");
	for (int i = 0; i < M; i++)
	{
		printf("%i ", ints[i]);
	}
	printf("\n");

    // send it back
    sendto(sockfd, buffer, N * sizeof(bool) + M * sizeof(int), 0,
           (struct sockaddr *) &cliaddr, len);
	
    // close the socket
	close(sockfd);
	printf("Server closed.\n");

	return 0;
}
