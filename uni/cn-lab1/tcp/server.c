#include <stdio.h>
#include <netdb.h>
#include <netinet/in.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>
#include <stdbool.h>

#define N 2
#define M 4
#define PORT 8080
#define SA struct sockaddr

// driver function
int main()
{
	int sockfd;
	int connfd;
	int len;
	struct sockaddr_in servaddr;
	struct sockaddr_in cli;

	// create and verify socket
	sockfd = socket(AF_INET, SOCK_STREAM, 0);
	if (sockfd == -1) {
		printf("Socket creation failed...\n");
		exit(0);
	}
	else
	{
		printf("Socket successfully created..\n");
	}

	memset(&servaddr, '\0', sizeof(servaddr));

	// assign IP, PORT
	servaddr.sin_family = AF_INET;
	servaddr.sin_addr.s_addr = htonl(INADDR_ANY);
	servaddr.sin_port = htons(PORT);

	// binding newly created socket to given IP and verification
	if ((bind(sockfd, (SA*)&servaddr, sizeof(servaddr))) != 0) 
	{
		printf("Socket bind failed...\n");
		exit(0);
	}
	else
	{
		printf("Socket successfully binded..\n");
	}

	// server is ready to listen and verification
	if ((listen(sockfd, 5)) != 0) 
	{
		printf("Listen failed...\n");
		exit(0);
	}
	else
	{
		printf("Server listening..\n");
	}

	len = sizeof(cli);

	// accept the data packet from client and verification
	connfd = accept(sockfd, (SA*)&cli, (unsigned int *) &len);
	if (connfd < 0) 
	{
		printf("Server accept failed...\n");
		exit(0);
	}
	else
	{
		printf("Server accept the client...\n");
	}

	// get data from the client
	bool bools[N];
	int ints[M];
	char buff[N * sizeof(bool) + M * sizeof(int)];

	read(connfd, buff, sizeof(buff));

	// send echo
	write(connfd, buff, sizeof(buff));

	// move data to type-specific arrays
	for (int i = 0; i < N; i++) 
	{
		memcpy(&bools[i], buff + i * sizeof(bool), sizeof(bool));
	}

	for (int i = 0; i < M; i++) 
	{
		memcpy(&ints[i], buff + N * sizeof(bool) + i * sizeof(int), sizeof(int));
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



	// close the socket
	close(sockfd);
	printf("Server closed.\n");

	return 0;
}
