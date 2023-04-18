// Client side implementation of UDP client-server model
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <stdio.h>
#include <time.h>
#include <stdbool.h>

#define N 2
#define M 4
#define PORT 8080
#define MAXLINE 1024

// driver code
int main() {
	int sockfd;
	struct sockaddr_in servaddr;

    srand((unsigned int) time(0));

	// Creating socket file descriptor
	if ((sockfd = socket(AF_INET, SOCK_DGRAM, 0)) < 0 ) {
		printf("socket creation failed\n");
		exit(EXIT_FAILURE);
	}

	memset(&servaddr, 0, sizeof(servaddr));
	
	// Filling server information
	servaddr.sin_family = AF_INET;
	servaddr.sin_port = htons(PORT);
	servaddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    bool bools[N];
    int ints[M];
	char buff[N * sizeof(bool) + M * sizeof(int)];

    // generate data
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

    // start time measurement
    clock_t t;
    t = clock();
	
    // send data
	sendto(sockfd, buff, sizeof(buff),
		0, (const struct sockaddr *) &servaddr,
			sizeof(servaddr));

    socklen_t len;
    char new_buff[N * sizeof(bool) + M * sizeof(int)];
    recvfrom(sockfd, new_buff, sizeof(new_buff), 0,
             (struct sockaddr *) &servaddr, &len);

    t = (clock() - t) / 2;
    double time_taken = ((double) t) / CLOCKS_PER_SEC;
    printf("Sending data took %f seconds.\n", time_taken);

    if (memcmp(buff, new_buff, N * sizeof(bool) + M * sizeof(int)) == 0)
    {
        printf("The data matches.\n");
    }

	close(sockfd);
    printf("Client closed.\n");

	return 0;
}
