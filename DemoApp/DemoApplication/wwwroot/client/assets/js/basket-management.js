const cookieObj = new URLSearchParams(document.cookie.replaceAll("&", "%26").replaceAll("; ", "&"))
var color = cookieObj.get("products")
console.log(color)