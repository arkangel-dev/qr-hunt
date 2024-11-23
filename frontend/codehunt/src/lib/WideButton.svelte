<script>
	export let Content = '';
	export let OnClick = null;
	export let Class = 'btn primary big showloader';

	export function ShowLoader() {
        button.classList.add('showloader')
	}

	export function HideLoader() {
        button.classList.remove('showloader')
	}

    let button = null
</script>

<button bind:this={button} class={Class} on:click={OnClick}>
	{Content}
	<img src="./LoadingIcon.png" alt="icon" class="loader" />
</button>
