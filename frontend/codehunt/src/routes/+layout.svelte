<script>
    import '../app.css';
	import { toasts, ToastContainer, FlatToast, BootstrapToast } from 'svelte-toasts';
</script>

<ToastContainer let:data>
	<FlatToast {data} />
</ToastContainer>
<slot/>
<style>
 
</style>