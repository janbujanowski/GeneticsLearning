
Vue.component('header-component', {
    props: ['menuItems'], // pass in the menu from the app
    template: '<header><ul><li v-for="item in menuItems">{{ item  }}</li</ul></header>'
});
