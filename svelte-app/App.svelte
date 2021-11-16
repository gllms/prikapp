<script>
    let test = "hoi";
    console.log(test);

    fetch('./locations.json')
        .then(response => response.json())
        .then(data => test = data)
        .catch(err => alert(err))
</script>

<p>Hello, World!</p>
<p>{test[0].Place}</p>