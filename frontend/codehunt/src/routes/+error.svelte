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

</script>

<div class="rootcontainer">
	<Header Title="Whoops" Subtitle="There is nothing at this page. Are you lost?" />

	<div class="container">
		<div class="banner-image">
			<img src="/InvalidCode.png" alt="CodeEntered" />
		</div>
	</div>

	<div class="filler"></div>
	<div class="container buttonset">
		<WideButton bind:this={button} Class="btn primary big" OnClick={() => {goto("../leaderboard/1")}} Content="Leaderboard" />
	</div>
</div>

<style>
	
</style>
