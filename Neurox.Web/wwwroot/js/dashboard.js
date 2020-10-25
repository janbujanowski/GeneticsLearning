// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

var app = new Vue({
    el: '#app',
    data: {
        message: 'Hello Vue!'
    }
});

new Vue({
    el: '#vue-app',
    data: function () {
        // parse the serialized data
        return JSON.parse('@data');
    }
});

// header-component.js
// in a real app you might use TypeScript or ECMAScript
// in a real app this would be a single file component probably

Vue.component('header-component', {
    props: ['menuItems'], // pass in the menu from the app
    template: '<header><ul><li v-for="item in menuItems">{{ item  }}</li</ul></header>'
});
