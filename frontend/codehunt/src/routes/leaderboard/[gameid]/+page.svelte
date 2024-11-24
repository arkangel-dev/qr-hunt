<script>
	import { MakeGetRequest } from '$lib/BackendComms';
	import Header from '$lib/Header.svelte';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';
	import WideButton from '$lib/WideButton.svelte';
	import { goto } from '$app/navigation';

	let dataloaded = false;
	let data = [];
	let timeRemaining = 0; // Initial time in seconds (45 minutes, 23 seconds)
	let timerDisplay = '--:--:--';
	let refreshButton = null;

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
		let gameid = $page.params.gameid;
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

	onMount(async () => {
		// Start the timer
		await refreshData();
	});
</script>

<div class="rootcontainer">
	<Header Title="Leaderboard" Subtitle="Lets see who’s winning this game!" />
	{#if dataloaded}
		<div class="leaderboard">
			{#each data as line}
				<div class="line {line.hasWon ? 'winner' : ''}">
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
		<WideButton Content="Logout" Class="btn big secondary" OnClick={() => goto("/logout")}/>
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
		gap: 1rem
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
		gap: 1rem;
		overflow: auto;
	}

	.leaderboard .line {
		color: #2e3b41;
		display: flex;
		justify-content: space-between;
		font-family: 'Inter';
		padding-left: 2rem;
		padding-right: 2rem;
	}

	.leaderboard .line.winner {
		background: #2e3b41;
		color: #c0c3b0;
		border-radius: 1rem;
		display: flex;
		align-items: center;
		padding-top: 1rem;
		padding-bottom: 1rem;
		box-shadow: inset 0px 0px 10px #0000007a !important;
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
