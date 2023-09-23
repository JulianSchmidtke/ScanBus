<script>
	import { onMount } from 'svelte';
	import imageBase64 from '$env/static/private/image.txt';

	let time = new Date();
	let modalOpen = false;

	onMount(() => {
		const interval = window.setInterval(() => {
			time = new Date();
		}, 250);
		return () => window.clearInterval(interval);
	});

	const handleIconClick = () => {
		modalOpen = true;
	};

	let fetchTimeout = -1;
	$: {
		if (!modalOpen) {
			if (fetchTimeout > 0) window.clearTimeout(fetchTimeout);
		} else {
			fetchTimeout = window.setTimeout(async () => {
				console.log('Sending img to api');

				var busNotice = {
					BusId: '1',
					Driver: '2',
					latitude: '51.954655',
					longitude: '7.6262226',
					utcTimeStamp: '1977-05-08T04:02:33.700Z',
					ImageContentType: 'media/jpeg',
					Base64Image: imageBase64
				};
				console.log(busNotice);
				await sendBusNotice();

				// TODO send img to api
			}, 4000);
		}
	}
</script>

<header>
	<div class="home-icon"><span class="mdi mdi-home" /></div>
	<div class="header-text">
		<h1>34 Ringlinie 🪦</h1>
		<h2>{time.toLocaleTimeString('de-DE')}</h2>
	</div>

	<div class="bustime-icon"><span class="mdi mdi-bus-clock" /></div>
</header>
<main>
	<div class="col1">
		<div class="mdi mdi-chevron-double-right" />
		<div class="col1-element" style="position: relative;">
			<div class="mdi mdi-menu-right" />
			<div class="mdi mdi-ticket" />
		</div>
		<div class="col1-element">
			<div class="mdi mdi-map" />
		</div>
		<div class="col1-element" style="background-color: #9B2353;" on:click={handleIconClick}>
			<div class="mdi mdi-car-off" />
		</div>
		<div class="col1-element" style="background-color: #2FA29B;">
			<div class="mdi mdi-email" />
		</div>
		<div class="col1-element" style="background-color: #E99437;">
			<div class="mdi mdi-phone" />
		</div>
		<div class="col1-element" style="background-color: #277DCA;">
			<div class="mdi mdi-sign-direction" />
		</div>
		<div class="col1-element" style="background-color: #E99437;">
			<div class="mdi mdi-bus-multiple" />
		</div>
	</div>
	<div class="col2">
		<h3>Ticketübersicht</h3>
		<div class="options-grid">
			<div style="background-color: #E99437;">
				<p>Einzelticket<br />&nbsp;</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #E99437;">
				<p>Zeitkarte Gültigkeit</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #E99437;">
				<p>Gruppe + MFK</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #E7D640; color: #223347;">
				<p>Zeitkarte Woche</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #E7D640; color: #223347;">
				<p>Zeitkarte Monat</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #E7D640; color: #223347;">
				<p>Zeitkarte<br /> Jahr</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #7EB03F;">
				<p>Verkaufs-<br />gültigkeit</p>
				<div class="mdi mdi-folder" />
			</div>
			<div style="background-color: #223347;" />
			<div style="background-color: #2FA29B;">
				<p>Letztes <br />Ticket</p>
			</div>
		</div>
	</div>
	<div class="col3">
		<div class="tickets">
			<h2>1x Fun Ticket</h2>
			<h4>Festpreis</h4>
			<h3>14,80 €</h3>
			<div>
				<div class="mdi mdi-arrow-left" />
				<div class="mdi mdi-pencil" />
				<div class="mdi mdi-cart-plus" />
			</div>
		</div>
		<div class="mdi mdi-printer">&nbsp;Drucken</div>
		<div class="paper">
			<label for="paper">Papier: </label>
			<progress id="paper" max="100" value="75">75%</progress>
		</div>
		<div class="more-buttons">
			<div class="mdi mdi-dots-horizontal" />
			<div
				class="mdi mdi-cash-refund"
				style="flex-grow: 1; background-color: grey; color: lightgrey;"
			/>
			<div class="mdi mdi-cancel" />
		</div>
	</div>

	<div class="modal" style:display={modalOpen ? 'block' : 'none'}>
		<div class="modal-content">
			<h1>Meldung abschicken?</h1>
			<img src="https://source.unsplash.com/random/400x300/?nature,plants,dark=" />
			<div class="buttons">
				<div class="notify-button {modalOpen ? 'animate-button' : ''}">Melden</div>
				<div class="cancel-button">Abbrechen</div>
			</div>
		</div>
	</div>
</main>

<style>
	header,
	main {
		background-color: #395a7b;
		color: white;
	}

	h1,
	h2 {
		margin: 0;
	}

	header {
		display: flex;
		justify-content: space-between;
		gap: 16px;
	}

	div.home-icon {
		height: 6rem;
		width: 6rem;
		background-color: #438ea5;
		display: flex;
		justify-content: center;
		align-items: center;
	}

	div.home-icon > span {
		font-size: 2.5rem;
	}

	div.bustime-icon {
		height: 6rem;
		width: 6rem;
		background-color: #38a9e5;
		display: flex;
		justify-content: center;
		align-items: center;
	}

	div.bustime-icon > span {
		font-size: 2.5rem;
	}

	.header-text {
		flex-grow: 1;
		display: flex;
		justify-content: space-between;
		align-items: center;
	}

	main {
		max-height: calc(100vh - 6rem - 16px);
		padding-block: 1rem;
		display: flex;
		justify-content: stretch;
		gap: 1rem;
	}

	.col1 {
		background-color: #3e7caf;
	}

	.col1 > .mdi-chevron-double-right {
		font-size: 2.5rem;
		text-align: center;
	}

	.col1 .col1-element {
		margin: 16px;
		padding-inline: 4rem;
		padding-block: 1.1rem;
		display: flex;
		justify-content: center;
		align-items: center;
		font-size: 2.25rem;
		background-color: #38a9e5;
	}

	.col1 .col1-element > .mdi-menu-right {
		position: absolute;
		left: -16px;
		background-color: transparent;
	}

	.col2 {
		flex-grow: 1.1;
		border: 2px solid #38a9e5;
		padding-inline: 16px;
		overflow-y: scroll;
		margin-bottom: 16px;
	}

	.col2 > .options-grid {
		display: grid;
		grid-template-columns: repeat(3, 1fr);
		grid-template-rows: repeat(3, 1fr);
		grid-column-gap: 16px;
		grid-row-gap: 16px;
		line-height: 1.25;
	}

	.options-grid > div {
		background-color: white;
		aspect-ratio: 1;
		font-size: 1.25rem;
	}

	.options-grid div p {
		margin: 16px;
	}

	.options-grid div .mdi {
		text-align: end;
		font-size: 4rem;
		margin-inline: 16px;
	}

	.col3 {
		flex-grow: 1.1;
		padding-right: 16px;
	}

	.col3 > .tickets {
		background-color: white;
		color: #405e80;
		padding: 16px;
	}

	.col3 > .tickets h4 {
		padding-block: 3rem;
	}

	.col3 > .tickets h3 {
		text-align: end;
		padding-top: 6rem;
	}

	.col3 > .tickets > div {
		display: flex;
		justify-content: space-between;
	}

	.col3 > .tickets > div div {
		color: white;
		background-color: #38a9e5;
		font-size: 1.25rem;
		padding: 1.5rem;
	}

	.col3 > .mdi-printer {
		margin-block: 16px;
		color: white;
		background-color: #38a9e5;
		font-size: 3rem;
		padding: 1.5rem;
		text-align: center;
	}

	.paper label {
		font-size: 0.75rem;
	}

	.more-buttons {
		display: flex;
		gap: 16px;
		justify-content: space-between;
	}

	.more-buttons .mdi {
		text-align: center;
		padding: 1.25rem;
		background-color: #38a9e5;
		color: white;
		font-size: 1.5rem;
	}

	.modal {
		position: fixed;
		z-index: 1;
		left: 0;
		top: 0;
		width: 100%;
		height: 100%;
		overflow: auto;
		background-color: rgba(0, 0, 0, 0.4);
	}

	.modal-content {
		position: relative;
		background-color: #395a7b;
		margin: auto;
		z-index: 10;
		top: 175px;
		padding: 16px;
		width: 40%;
		box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
		animation-name: animatetop;
		animation-duration: 0.4s;
	}

	.modal-content h1 {
		padding-bottom: 16px;
	}

	.modal-content img {
		display: block;
		width: 100%;
		margin: 0 auto;
	}

	.modal-content .buttons {
		margin-top: 1rem;
		display: flex;
		justify-content: space-between;
	}

	.modal-content .buttons .notify-button {
		position: relative;
		display: inline-block;
		z-index: 10;
		padding: 1rem;
		padding-inline: 3.5rem;
		outline: 1px solid #9b2353;
	}

	.modal-content .buttons .notify-button::before {
		content: '';
		position: absolute;
		background-color: #9b2353;
		z-index: 5;
		top: 0;
		left: 0;
		right: 100%;
		bottom: 0;
		transition: right 4s ease-in;
	}

	.modal-content .buttons .notify-button.animate-button::before {
		right: 0;
	}

	.modal-content .buttons .cancel-button {
		display: inline-block;
		padding: 1rem;
		background-color: #38a9e5;
	}

	@keyframes animatetop {
		from {
			top: -300px;
			opacity: 0;
		}
		to {
			top: 175px;
			opacity: 1;
		}
	}
</style>
