<script>
	import FoundCodeDisplay from '$lib/FoundCodeDisplay.svelte';
	import Header from '$lib/Header.svelte';
	import { toasts, ToastContainer, FlatToast, BootstrapToast } from 'svelte-toasts';
	import { Register, DoIHaveAValidToken } from '$lib/BackendComms.js';
	import WideButton from '$lib/WideButton.svelte';
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';

	let loggedIn = false;
	let alreadyScanned = false;
	let name = '';
	let phonenumber = '';

	let button = null;
	let redirectUrl = '';

	onMount(() => {
		const params = new URLSearchParams(window.location.search);
		redirectUrl = params.get('redirect') ?? './leaderboard';

		if (DoIHaveAValidToken()) {
			console.log("Redirect")
			goto(redirectUrl);
		}
	});

	async function OnClick() {
		// toasts.add({
		// 	title: 'Success',
		// 	description: 'Your account has been registered',
		// 	duration: 1000000, // 0 or negative to avoid auto-remove
		// 	placement: 'top-center',
		// 	theme: 'dark',
		// 	type: 'success'
		// });

		button.ShowLoader();
		if (await Register(phonenumber.toString(), name.toString())) {
			goto(redirectUrl);
		}
		button.HideLoader();
	}
</script>

<div class="rootcontainer">
	<Header Title="Register" Subtitle="Welcome! Enter the following fields so you can get started!" />

	<div class="container">
		<div class="banner-image">
			<img src="./LoginBanner.png" alt="CodeEntered" />
		</div>
	</div>

	<div class="filler"></div>

	<div class="container input-container">
		<input bind:value={name} type="text" placeholder="Enter your name" class="large-input" />
		<input
			bind:value={phonenumber}
			type="number"
			placeholder="Enter your phone number"
			class="large-input"
		/>
	</div>
	<div class="container buttonset">
		<WideButton bind:this={button} Class="btn primary big" {OnClick} Content="Register" />
		
		<WideButton bind:this={button} Class="btn primary-lowprofile big" OnClick={() => {goto("./login")}} Content="Login" />
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
