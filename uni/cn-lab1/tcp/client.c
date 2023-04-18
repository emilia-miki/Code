#include <arpa/inet.h>
#include <netdb.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <unistd.h>
#include <stdbool.h>
#include <time.h>

#define N 2
#define M 4
#define PORT 8080
#define SA struct sockaddr

int main()
{
	int sockfd;
	struct sockaddr_in servaddr;

    srand((unsigned int) time(0));

	// socket create and verification
	sockfd = socket(AF_INET, SOCK_STREAM, 0);
	if (sockfd == -1) 
    {
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
	servaddr.sin_addr.s_addr = inet_addr("127.0.0.1");
	servaddr.sin_port = htons(PORT);

	// connect the client socket to server socket
	if (connect(sockfd, (SA*)&servaddr, sizeof(servaddr)) != 0) 
    {
		printf("connection with the server failed...\n");
		exit(0);
	}
	else
    {
		printf("connected to the server..\n");
    }

	// generate data
	bool bools[N];
    int ints[M];
	char buff[N * sizeof(bool) + M * sizeof(int)];

    for (int i = 0; i < N; i++) 
    {
        bools[i] = rand() % 2;
    }

    for (int i = 0; i < M; i++) 
    {
        ints[i] = rand();
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

    // write data to a char array
    for (int i = 0; i < N; i++) 
    {
        memcpy(buff, bools, N * sizeof(bool));
    }

    for (int i = 0; i < M; i++) 
    {
        memcpy(buff + N * sizeof(bool), ints, M * sizeof(int));
    }
	
	char new_buff[N * sizeof(bool) + M * sizeof(int)];

	// start time measurement
	clock_t t = clock();

    // send data
    write(sockfd, buff, sizeof(buff));

	// get data back
	read(sockfd, new_buff, sizeof(new_buff));

	t = (clock() - t) / 2;
	double time_taken = ((double) t) / CLOCKS_PER_SEC;
	printf("Sending data took %f seconds.\n", time_taken);

	if (memcmp(buff, new_buff, N * sizeof(bool) + M * sizeof(int)) == 0)
	{
		printf("The data matches.'\n");
	}

	// close the socket
	close(sockfd);
    printf("Client closed.\n");

	return 0;
}
