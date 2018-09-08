const hljs = require('highlight.js');

/**
 * @typedef {{result: string, language: string}} HighlightResult
 */

function highlight(callback, source, lang) {
    /** @type {hljs.IHighlightResultBase} */
    let result;
    try {
        if (lang && hljs.getLanguage(lang)) {
            result = hljs.highlight(lang, source, true);
        } else {
            result = hljs.highlightAuto(source);
        }
        callback(null, { result: result.value, language: result.language });
    } catch (e) {
        callback(e, { result: source, language: lang });
    }
}

module.exports = highlight;