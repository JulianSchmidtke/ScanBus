<script>
	import { onMount } from 'svelte';
	import mock from '../../../static/mockup_new.json?raw';
	const mockObject = JSON.parse(mock);

	onMount(async () => {
		const L = (await import('leaflet')).default;
		const map = L.map('map').setView([mockObject.latitude, mockObject.longitude], 14);
		L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
			attribution:
				'&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
		}).addTo(map);
		var marker = L.marker([mockObject.latitude, mockObject.longitude]).addTo(map);

		let layer = null;
		window.setInterval(async () => {
			try {
				const geojsonResponse = await fetch('https://rest.busradar.conterra.de/prod/fahrzeuge');
				const geojson = await geojsonResponse.json();

				if (layer !== null) layer.clearLayers();
				layer = L.geoJSON(geojson, {
					// @ts-ignore
					pointToLayer: (feature, latlng) => {
						return new L.Marker(latlng, {
							icon: new L.DivIcon({
								className: 'busses',
								html:
									'<div><div class="mdi mdi-bus" /><div class="text">' +
									feature.properties.linientext +
									'</div></div>'
							})
							// radius: 8,
							// fillColor: '#0f0',
							// color: '#000',
							// weight: 1,
							// opacity: 1,
							// fillOpactiy: 1
						});
					}
				}).addTo(map);
			} catch (Error) {}
		}, 15_000);
	});
</script>

<svelte:head>
	<link
		rel="stylesheet"
		href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"
		integrity="sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY="
		crossorigin=""
	/>
</svelte:head>

<div class="wrapper">
	<aside>
		<div id="map" />
		<!-- <iframe
			style="display: block; border: none;"
			width="1194"
			height="200"
			src="https://www.openstreetmap.org/export/embed.html?bbox=7.634629011154176%2C51.9515259713887%2C7.641710042953492%2C51.95491474004837&amp;layer=mapnik&amp;marker=51.95322%2C7.6381699999999455"
		/> -->
	</aside>
	<div>
		<main>
			<h1 style="position: relative;">
				<span
					class="mdi mdi-arrow-left-thin"
					style="position: absolute; left: 16px; background-color: #438ea5; padding-inline: 1rem;"
				/>Meldung vom {new Date(mockObject.timestamp).toLocaleString('de-DE')}
			</h1>
			<h2>Linie {mockObject.busId} - Fahrer: {mockObject.driver}</h2>
			<div class="actions">
				<div class="mdi mdi-check" style="background-color: #7EB03F;" />
				<div class="mdi mdi-trash-can" style="background-color: #9B2353;" />
				<div class="mdi mdi-dots-horizontal" style="background-color: #38a9e5;" />
			</div>
			<section>
				<h3>Adresse</h3>
				<p>
					{mockObject.address.road}
					{mockObject.address.house_number}, {mockObject.address.postcode}
					{mockObject.address.city}
				</p>
			</section>
			<section>
				<h3>Meldungsdetails</h3>
				<p style="text-align: center;">
					Fahrzeug vom Typ <strong>{mockObject.type}</strong> mit amtlichem Kennzeichen
					<strong>{mockObject.licensePlate}</strong>
				</p>
				<div class="analysis-images">
					{#each mockObject.images as img}
						<figure>
							<img src="data:image/jpeg;base64, {img.image}" />
							<caption>{img.type}</caption>
						</figure>
					{/each}
				</div>
				<details>
					<img src="data:image/jpeg;base64, {mockObject.base64Image}" />
					<summary>Foto wie gemeldet</summary>
				</details>
			</section>
		</main>
	</div>
</div>

<style>
	:global(body) {
		background-color: #223347;
	}

	div.wrapper {
		color: white;
		background-color: #223347;
	}

	div main {
		background-color: #395a7b;
		margin: 0 auto;
		padding: 16px;
		width: 60%;
	}

	h1,
	h2,
	h3 {
		margin: 0;
		padding: 0;
		font-weight: normal;
		text-align: center;
	}

	h1 {
		line-height: 1.5;
	}

	section {
		outline: 1px solid lightgrey;
		padding: 8px;
		margin-inline: 16px;
		margin-bottom: 16px;
		background-color: white;
		color: #395a7b;
	}

	img {
		max-width: 100%;
		height: auto;
	}

	figure {
		margin: 0;
	}

	figure caption {
		display: block;
		text-align: left;
		font-size: 0.9rem;
		font-family: monospace;
		padding-block: 0.35rem;
	}

	div.analysis-images {
		display: flex;
		gap: 1rem;
		justify-content: space-between;
		align-items: stretch;
		padding-block: 16px;
	}

	div.analysis-images figure {
		width: 150px;
		flex-grow: 1;
		text-align: center;
	}

	div.analysis-images figure img {
		width: auto;
		height: 200px;
	}

	div.analysis-images figure caption {
		text-align: center;
	}

	.actions {
		display: flex;
		justify-content: start;
		gap: 0.5rem;
		padding: 16px;
	}

	.actions div {
		display: flex;
		justify-content: center;
		align-items: center;
		font-size: 1.5rem;
		height: 2.5rem;
		width: 2.5rem;
	}

	#map {
		width: 1194px;
		height: 250px;
	}

	:global(.busses) {
		position: relative;
		color: black;
		font-size: 1rem;
	}

	:global(.busses .text) {
		position: absolute;
		color: black;
		font-size: 0.7rem;
		font-family: monospace;
		top: -12px;
		left: 2px;
	}
</style>
