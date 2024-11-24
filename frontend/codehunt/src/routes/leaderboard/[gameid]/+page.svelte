<script>
	import { GetWebSocketUrl, MakeGetRequest } from '$lib/BackendComms';
	import Header from '$lib/Header.svelte';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import WideButton from '$lib/WideButton.svelte';
	import { goto } from '$app/navigation';
	import { flip } from "svelte/animate";

	let dataloaded = false;
	let data = [];
	let timeRemaining = 0; // Initial time in seconds (45 minutes, 23 seconds)
	let timerDisplay = '--:--:--';
	let refreshButton = null;
	let gameid = ''

	// Format seconds into HH:MM:SS
	function formatTime(seconds) {
		const hours = Math.floor(seconds / 3600)
			.toString()
			.padStart(2, '0');
		const minutes = Math.floor((seconds % 3600) / 60)
			.toString()
			.padStart(2, '0');
		const secs = Math.floor(seconds % 60)
			.toString()
			.padStart(2, '0');
		return `${hours}:${minutes}:${secs}`;
	}

	let timerInterval = null;
	let isAuthenticated = false;
	// Countdown timer
	function startTimer() {
		if (timerInterval) clearInterval(timerInterval);
		timerInterval = setInterval(() => {
			if (timeRemaining > 0) {
				timeRemaining--;
				timerDisplay = formatTime(timeRemaining);
			} else {
				clearInterval(timerInterval);
				timerDisplay = '00:00:00';
			}
		}, 1000);
	}

	async function refreshData() {
		refreshButton.ShowLoader();
		gameid = $page.params.gameid;
		let result = await MakeGetRequest(`/Api/${gameid}/GetLeaderboard`);
		let rjson = await result.json();

		data = rjson.leaderboard;
		timeRemaining = rjson.timeRemaining;
		startTimer();
		dataloaded = true;
		setTimeout(() => {
			refreshButton.HideLoader();
		}, 500);
	}

	

	// Method to create a WebSocket connection and listen for SCOREUPDATED
	async function connectWebSocket(url) {
		return new Promise((resolve, reject) => {
			const socket = new WebSocket(url);
			socket.onopen = () => console.log("Connection opened");
			socket.onerror = (err) => console.error("WebSocket error:", err);
			socket.onclose = () => console.log("Connection closed");
			// Open WebSocket connection
			socket.onopen = () => {
				console.log('WebSocket connection established.');
				resolve(socket); // Resolve when connection is established
			};

			// Listen for messages in the background
			socket.onmessage = async (event) => {
				console.log('Message from server:', event.data);
				if (event.data === 'SCOREUPDATED') {
					await refreshData(); // Call the method when the specific message is received
				}
			};

			// Handle errors
			socket.onerror = (error) => {
				console.error('WebSocket error:', error);
				reject(error);
			};

			// Handle closure
			socket.onclose = () => {
				console.log('WebSocket connection closed.');
			};
		});
	}

	// Asynchronous wrapper to establish the connection
	async function initiateWebSocket() {
		try {
			
			const url = GetWebSocketUrl(`/Api/${gameid}/GetLeaderboard/ws`); // Replace with your actual WebSocket URL
			console.log(`Connecting to ${url}`)
			const socket = await connectWebSocket(url);
			console.log('Connection established. WebSocket will continue listening for messages.');
			return socket; // Return the WebSocket instance immediately
		} catch (error) {
			console.error('Failed to establish WebSocket connection:', error);
			throw error; // Ensure the caller is aware of the failure
		}
	}



	onMount(async () => {
		// Start the timer
		await refreshData();
		await initiateWebSocket();
		isAuthenticated = localStorage.getItem('token')
	});
</script>

<div class="rootcontainer">
	<Header Title="Leaderboard" Subtitle="Lets see who’s winning this game!" />
	{#if dataloaded}
		<div class="leaderboard">
			{#each data as line (line.name)}
				<div animate:flip="{{ duration: 600 }}" class="line {line.hasWon ? 'winner' : ''}">
					<h1>
						{line.name}
						{#if line.hasWon}
							<img src="/Crown.svg" alt="crown" />
						{/if}
					</h1>
					<h2>{line.count}</h2>
				</div>
			{/each}
		</div>
		<div class="filler"></div>
		<div class="timer-container">
			<h1>Time Remaining</h1>
			<p>{timerDisplay}</p>
		</div>
	{:else}
		<div class="loading">
			<p>Loading...</p>
		</div>

		<div class="filler"></div>
	{/if}

	<div class="container vertical">
		{#if (isAuthenticated)} 
			<WideButton Content="Logout" Class="btn big secondary" OnClick={() => goto('/logout')} />
		{/if}
		<WideButton
			bind:this={refreshButton}
			Content="Refresh"
			Class="btn primary big"
			OnClick={refreshData}
		/>
	</div>
</div>

<style>
	.container.vertical {
		display: flex;
		flex-direction: row !important;
		margin-bottom: 2rem;
		gap: 1rem;
	}

	:global(.container.vertical button:nth-child(1)) {
		flex: 1 1 auto;
		width: 50%;
	}

	:global(.container.vertical button:nth-child(2)) {
		flex: 1 1 auto;
		width: 50%;
	}

	.leaderboard {
		display: flex;
		flex-direction: column;
		padding: 1rem 2rem;
		margin-bottom: 3rem;
		gap: .75rem;
		overflow: auto;
	}

	.leaderboard .line {
		color: #2e3b41;
		display: flex;
		justify-content: space-between;
		font-family: 'Inter';
		padding-left: 1.5rem;
		padding-right: 1.5rem;
		border-radius: 1rem;

		padding-top: .75rem;
		padding-bottom: .75rem;
		background: #f2f6da;
	}

	.leaderboard .line.winner {
		background: #2e3b41;
		color: #f2f6da;
		display: flex;
		align-items: center;	padding-top: 1rem;
		box-shadow: 0px 0px 10px #0000007a !important;
		padding-bottom: 1rem;
	}

	.leaderboard .line.winner img {
		width: 1.25rem;
		margin-left: 0.5rem;
	}

	.leaderboard .line h1 {
		font-size: 1.25rem;
		margin: 0;
	}

	.leaderboard .line h2 {
		font-size: 1.25rem;
		margin: 0;
		font-weight: 400;
	}

	.loading {
		width: 100%;
		display: flex;
		justify-content: center;
	}

	.loading p {
		font-family: 'Inter';
		font-size: 2rem;
		color: #2e3b41;
	}

	.timer-container {
		width: 100%;
		display: flex;
		flex-direction: column;
		align-items: center;
	}

	.timer-container h1 {
		margin: 0px;
		font-family: Inter;
		color: #00000079;
		font-weight: 500;
		font-size: 1rem;
	}

	.timer-container p {
		font-family: 'Inter';
		margin-top: 0.5rem;
		font-size: 2.5rem;
		font-weight: 500;
		margin-bottom: 1.5rem;
		color: var(--secondary-color);
		letter-spacing: 2px;
	}
</style>
