<script>
	import { goto } from '$app/navigation';
	import FoundCodeDisplay from '$lib/FoundCodeDisplay.svelte';
	import Header from '$lib/Header.svelte';

	let loggedIn = false;
	let alreadyScanned = false;
	let state = 'INVALID_CODE';

	let title = '';
	let subtitle = '';
	let image = '';

	switch (state) {
		case 'GAME_ENDED':
			image = './SadSherlock.png';
			title = 'Game Ended';
			subtitle = 'Someone else has already won. Better luck next time!';
			break;

		case 'YOU_WON':
			image = './PartySherlock.png';
			title = 'You Won!';
			subtitle = 'You found the minimum number of codes needed to win!';
			break;

		case 'TIMES_UP':
			image = './TimesUp.png';
			title = 'Times Up!';
			subtitle = 'The time allocated for this game is up';
			break;

		case 'INVALID_CODE':
			image = './InvalidCode.png';
			title = 'Invalid Code';
			subtitle = 'The code you scanned does not appear to be a valid code';
			break;

		case 'CODE_FOUND':
			image = './CodeEntered.png';
			title = 'You found a code!';
			subtitle = loggedIn
				? 'It has been added to your leader board! Now go find more codes!'
				: 'This code is part of a scavenger hunt! But first you need sign in!';
			break;

		case 'CODE_ALREADY_SCANNED':
			image = './SadSherlock.png';
			title = 'You already got this one!!';
			subtitle = 'This code has already been scanned by you! Go find more!';
			break;
	}
</script>

<div class="rootcontainer">
	<Header Title={title} Subtitle={subtitle} />

	<div class="container">
		<div class="banner-image">
			<img src={image} alt="CodeEntered" />
		</div>
	</div>
	<FoundCodeDisplay />

	<div class="filler"></div>

	<div class="container buttonset">
		{#if !loggedIn}
			<button on:click={goto('/register')} class="btn primary big">Register</button>
			<button on:click={goto('/login')} class="btn secondary big">Login</button>
		{:else}
			<button class="btn primary big">Leaderboard</button>
		{/if}
	</div>
</div>

<style>
	.rootcontainer {
		display: flex;
		flex-direction: column;
		height: 100dvh;
	}

	.rootcontainer .filler {
		flex: 1 0 auto;
	}
</style>
