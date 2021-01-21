const path = require('path');

module.exports = {
    darkMode: 'class',
    purge: [ path.resolve(__dirname, '../**/Pages/*.fs')],
    theme: {
        extend: {
            colors: {
                'z-gray': '#666',
            },
        },
    },
};
