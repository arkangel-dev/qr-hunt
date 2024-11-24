<script>
	import { goto } from '$app/navigation';
	import { MakePostRequest, DoIHaveAValidToken } from '$lib/BackendComms';
	import FoundCodeDisplay from '$lib/FoundCodeDisplay.svelte';
	import Header from '$lib/Header.svelte';
	import { onMount } from 'svelte';
	import { toasts } from 'svelte-toasts';
	import { page } from '$app/stores';

	let loggedIn = false;
	let alreadyScanned = false;

	let title = '';
	let subtitle = '';
	let image = '';
	let loadComplete = false;
	let token = '';
	let showCode = true;
	let gameid = '';

	onMount(async () => {
		loggedIn = DoIHaveAValidToken();

		gameid = $page.params.gameid;
		token = $page.params.code;
		let state = 'INVALID_CODE';

		if (!gameid || !token) {
			state = 'INVALID_CODE';
		} else {
			let result = await MakePostRequest(`/Api/${gameid}/Scan?code=${token}`);
			let rjson = await result.json();
			state = rjson.statusCode;
			loggedIn = rjson.isAuthenticated;
		}

		switch (state) {
			case 'GAME_ENDED':
				showCode = false;
				image = '/SadSherlock.png';
				title = 'Game Ended';
				subtitle = 'Someone else has already won. Better luck next time!';
				break;

			case 'YOU_WON':
				showCode = false;
				image = '/PartySherlock.png';
				title = 'You Won!';
				subtitle = 'You found the minimum number of codes needed to win!';
				break;

			case 'TIMES_UP':
				showCode = false;
				image = '/TimesUp.png';
				title = 'Times Up!';
				subtitle = "The time allocated for this game is up. You'll get em next time champ!";
				break;

			case 'INVALID_CODE':
				image = '/InvalidCode.png';
				title = 'Invalid Code';
				subtitle = 'The code you scanned does not appear to be a valid code';
				break;

			case 'CODE_FOUND':
				image = '/CodeEntered.png';
				title = loggedIn ? 'Code Scanned!' : 'You found a code!';
				subtitle = loggedIn
					? 'It has been added to your leader board! Now go find more codes!'
					: 'This code is part of a scavenger hunt! But first you need sign in!';
				if (loggedIn) {
					toasts.add({
						title: 'Code Scanned Successfully',
						description: 'This code has been successfully added to your score!',
						duration: 5000,
						placement: 'top-center',
						theme: 'dark',
						type: 'success'
					});
				}
				break;

			case 'CODE_ALREADY_SCANNED':
				image = '/SadSherlock.png';
				title = 'You already got this one!!';
				subtitle = 'This code has already been scanned by you! Go find more!';
				break;
		}
		loadComplete = true;
	});

	function gotoWithRedirect(url) {
		const currentRelativeUrl = window.location.pathname + window.location.search;
		const updatedUrl = `${url}?redirect=${encodeURIComponent(currentRelativeUrl)}`;
		goto(updatedUrl);
	}
</script>

<div class="rootcontainer">
	{#if loadComplete}
		<Header Title={title} Subtitle={subtitle} />
		<div class="container">
			<div class="banner-image">
				<img src={image} alt="CodeEntered" />
			</div>
		</div>
		{#if showCode}
			<FoundCodeDisplay Code={token} />
		{/if}

		<div class="filler"></div>

		<div class="container buttonset">
			{#if !loggedIn}
				<button on:click={() => gotoWithRedirect('/register')} class="btn primary big"
					>Register</button
				>
				<button on:click={() => gotoWithRedirect('/login')} class="btn secondary big">Login</button>
			{:else}
				<button on:click={() => goto(`/leaderboard/${gameid}`)} class="btn primary big"
					>Leaderboard</button
				>
			{/if}
		</div>
	{/if}
</div>

<style>
</style>
