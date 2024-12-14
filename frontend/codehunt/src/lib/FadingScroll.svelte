<script>
	import { onMount } from "svelte";

    let inst = null
    let top = null;
    let bottom = null;

    onMount(() => {
        
    })

    let scrollChange = (eventData) => {
        

        var fheight = inst.scrollTop + inst.offsetHeight 

        // console.log({
        //     top: inst.scrollTop,
        //     sheight: fheight,
        //     height: inst.offsetHeight ,
        // })

        if (fheight + 20 >= inst.scrollHeight) {
            bottom.classList.remove('visible')
        } else {
            bottom.classList.add('visible')
        }

        if (inst.scrollTop == 0) {
            top.classList.remove('visible')
        } else {
            top.classList.add('visible')
        }
    }


</script>

<div  class="overflowscroll">
	<div bind:this={top} class="faders topscroll "></div>
	<div bind:this={inst} on:scroll={scrollChange} class="overflowinner">
		<slot />
	</div>
	<div bind:this={bottom} class="faders visible bottomscroll"></div>
</div>

<style>
	.faders {
		height: 5rem;
		width: 100%;
		position: absolute;
        pointer-events: none;
        opacity: 0;
        transition: opacity .25s linear;
	}

    :global(.faders.visible) {
        opacity: 1;
    }

	.topscroll {
        top: 0;
		background: rgb(192, 195, 176);
		background: linear-gradient(180deg, rgba(192, 195, 176, 1) 9%, rgba(255, 255, 255, 0) 100%);
	}

	.bottomscroll {
		background: rgb(192, 195, 176);
		background: linear-gradient(0deg, rgba(192, 195, 176, 1) 9%, rgba(255, 255, 255, 0) 100%);
        bottom: 0;
	}

	.overflowscroll {
        min-height: 1px;
		position: relative;
		/* display: flex;
        flex-direction: column; */
	}

	.overflowinner {
		overflow: auto;
		max-height: 100%;
		z-index: -1;
	}

	.overflowinner::-webkit-scrollbar {
		width: 0.5rem;
	}

	.overflowinner::-webkit-scrollbar-track {
		background-color: #f2f6da2f;
		-webkit-border-radius: 10px;
		border-radius: 10px;
	}

	.overflowinner::-webkit-scrollbar-thumb {
		-webkit-border-radius: 10px;
		border-radius: 10px;
		background: #2e3b41;
	}
</style>
