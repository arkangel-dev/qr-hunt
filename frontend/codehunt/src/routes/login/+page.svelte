<script>
	import FoundCodeDisplay from '$lib/FoundCodeDisplay.svelte';
	import Header from '$lib/Header.svelte';
	import { toasts, ToastContainer, FlatToast, BootstrapToast } from 'svelte-toasts';
	import { Login, DoIHaveAValidToken } from '$lib/BackendComms.js';
	import WideButton from '$lib/WideButton.svelte';
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';

	let loggedIn = false;
	let alreadyScanned = false;
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
		button.ShowLoader();
		if (await Login(phonenumber)) {
			goto(redirectUrl);
		}
		button.HideLoader();
	}
</script>

<div class="rootcontainer">
	<Header Title="Login" Subtitle="Welcome back! You can use your player code to log back in!" />

	<div class="container">
		<div class="banner-image">
			<img src="./LoginBanner.png" alt="CodeEntered" />
		</div>
	</div>

	<div class="filler"></div>

	<div class="container input-container">
		<input
			bind:value={phonenumber}
			type="number"
			placeholder="Enter your phone number"
			class="large-input"
		/>
	</div>
	<div class="container buttonset">
		<WideButton bind:this={button} Class="btn primary big" {OnClick} Content="Login" />
		<WideButton bind:this={button} Class="btn primary-lowprofile big" OnClick={() => {goto("./register")}} Content="Register" />
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
