<script>
	let time = new Date();
</script>

<header>
	<div class="home-icon"><span class="mdi mdi-home" /></div>
	<div class="header-text">
		<h1>34 Ringline 🪦</h1>
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
		<div class="col1-element" style="background-color: #9B2353;">
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
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
			<div />
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
		<div class="mdi mdi-printer">Drucken</div>
		<div class="paper">
			<label for="paper">Papier: </label>
			<progress id="paper" max="100" value="50">0%</progress>
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
		min-height: calc(100vh - 4rem);
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
		padding-block: 2rem;
		display: flex;
		justify-content: center;
		align-items: center;
		font-size: 2.5rem;
		background-color: #38a9e5;
	}

	.col1 .col1-element > .mdi-menu-right {
		position: absolute;
		left: -16px;
		background-color: transparent;
	}

	.col2 {
		flex-grow: 1.3;
		border: 2px solid #38a9e5;
		padding-inline: 16px;
	}

	.col2 > .options-grid {
		display: grid;
		grid-template-columns: repeat(3, 1fr);
		grid-template-rows: repeat(4, 1fr);
		grid-column-gap: 16px;
		grid-row-gap: 16px;
	}

	.options-grid div {
		background-color: white;
		aspect-ratio: 1;
	}

	.col3 {
		flex-grow: 1;
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
</style>
