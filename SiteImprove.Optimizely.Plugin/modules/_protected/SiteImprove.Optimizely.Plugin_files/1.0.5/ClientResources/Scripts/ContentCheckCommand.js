define([
    'dojo/_base/declare',
    'dojo/when',
    'dojo/request',
    'epi/shell/command/_Command',
    'epi/shell/_ContextMixin',
    'epi-cms/command/_NonEditViewCommandMixin',
    'siteimprove/jszip.min',
    'siteimprove/FileSaver.min'
], function (
    declare,
    when,
    request,
    _Command,
    _ContextMixin,
    _NonEditViewCommandMixin,
    JSzip,
    FileSaver
) {
    return declare([_Command, _ContextMixin, _NonEditViewCommandMixin, JSzip], {
        iconClass: 'siteimprove-icon',
        canExecute: true,
        package: [],
        label: 'Content Check',
        _execute: function () {
            var scope = this;
            when(scope.getCurrentContext(), function (context) {
                if (!context || !context.hasTemplate || !context.capabilities) return;

                if (!context.capabilities.isPage && !context.capabilities.isBlock) return;

                const path = context.previewUrl;

                // Try fetching the content
                fetch(window.location.origin + path)
                    .then((resp) => {
                        if (resp.ok) {
                            return resp.text();
                        }
                    }).then(html => {
                        if (context.capabilities.isPage) {
                            request
                                .get(window.epi.routes.getActionPath({ moduleArea: "SiteImprove.Optimizely.Plugin", controller: "Siteimprove", action: "pageUrl" }), {
                                    query: {
                                        contentId: context.id,
                                        locale: context.language
                                    },
                                    handleAs: 'json',
                                })
                                .then((response) => {
                                    scope.pushHtml(html, response.isDomain ? '' : response.url);
                                });
                        } else {
                            scope.pushHtml(html, '');
                        }
                    });
            });
        },
        /**
         * Zip HTML & Assets for delivery to Siteimprove
         * @param {HTML} html HTML content to process
         */
        _findAndGenerateAssets: function (html = '') {
            if (!html.trim().length) {
                throw new Error("Siteimprove: Missing HTML for parsing");
            }

            // Refs
            const
                oParser = new DOMParser(),
                scope = this,
                domain = window.location.origin;

            // Properties
            let
                strHTML = html,
                markup = oParser.parseFromString(strHTML, 'text/html'),
                arrAssets = Array.from(markup.querySelectorAll('script[src], link[href]')).filter(x => {
                    const source = typeof x.src !== 'undefined' ? x.src : x.href;
                    return (!source.includes('EPiServer') && !source.includes('Util')) && (source.includes('.js') || source.includes('.css'));
                }),
                getAssetProperties = function (path, domain) {
                    let file = '';

                    // Remove location 
                    file = path.replace(domain, '');
                                        
                    return {
                        'zip': file.substring(1), // Remove first "/" to avoid clashes in zip
                        'original': file,
                        'relative': '.' + file
                    };
                };

            // Start fethcing assets + adjusting markup
            let arrPromises = [];
            arrAssets.forEach(asset => {
                const
                    source = typeof asset.src !== 'undefined' ? asset.src : asset.href,
                    file = getAssetProperties(source, domain);

                // Replace path with relative path
                strHTML = strHTML.replace(file.original, file.relative);

                // Grab resource
                arrPromises.push(
                    fetch(source)
                        .then(resp => {
                            if (resp.ok) {
                                const data = resp.blob();

                                // Add file to package
                                scope.package.push({
                                    'filename': file.zip,
                                    'content': data
                                });
                            } else {
                                throw new Error(resp.status + " Failed Fetch for" + source);
                            }
                        })
                        .catch(resp => console.log("Siteimprove: Issue with feching ressource", resp))
                );
            });

            // Add html as index
            scope.package.push({
                'filename': 'index.html',
                'content': strHTML
            });

            // Wait for all resources has been fetched and added to queue
            return Promise.all(arrPromises);
        },
        pushHtml: function (html, pageUrl) {
            var self = this;
            request
                .get(window.epi.routes.getActionPath({ moduleArea: "SiteImprove.Optimizely.Plugin", controller: "Siteimprove", action: "token" }), { handleAs: 'json' })
                .then(function (token) {
                    var si = window._si || [];
                    si.push([
                        'onHighlight',
                        function (highlightInfo) {
                            // Highlight is running again before previous highlight was cleaned up. Clean it up now!
                            if (window.siteimproveHighlightCleanupFunction) {
                                window.siteimproveHighlightCleanupFunction();
                            }

                            var iframe = document.querySelector('iframe[name="sitePreview"]');
                            var idocument = iframe.contentWindow.document;

                            // Add styling to iFrame
                            var stylingId = 'siteimprove-styling';
                            var styling = idocument.getElementById(stylingId);
                            if (!styling) {
                                idocument.body.insertAdjacentHTML(
                                    'beforeend',
                                    `
              <style type="text/css" id="${stylingId}">
                  .siteimprove-highlight {
                    animation: siteimprove-pulse 0.5s;
                    animation-iteration-count: 8;
                    outline: 5px solid transparent;
                    outline-offset: -3px;
                  }
                  .siteimprove-highlight-inner {
                    animation: siteimprove-pulse 0.5s;
                    animation-iteration-count: 8;
                    outline: 5px solid transparent;
                    outline-offset: -5px;
                  }
                  @keyframes siteimprove-pulse {
                    from { outline-color: transparent; }
                    50% { outline-color: #ffc107; }
                    to { outline-color: transparent; }
                  }
              </style>
            `
                                );
                            }

                            // Highlight classes
                            var highlightClass = 'siteimprove-highlight';
                            var highlightClassInner = 'siteimprove-highlight-inner';

                            // Add highlight
                            highlightInfo.highlights.forEach((info, index) => {
                                // If error is inside the HEAD tag. Then Highlight the body
                                if (info.selector.startsWith('HEAD')) {
                                    info.selector = 'BODY';
                                    info.offset = null;
                                }

                                var element = idocument.querySelector(info.selector);
                                if (element) {
                                    // Scroll into view
                                    if (index === 0) {
                                        element.scrollIntoView({
                                            behavior: 'smooth',
                                            block: 'center',
                                        });
                                    }

                                    // Cleanup after adding highlight
                                    var cleanup = (callback) => {
                                        window.siteimproveHighlightCleanupFunction = () => {
                                            callback();
                                            window.clearTimeout(
                                                window.siteimproveHighlightCleanupTimer
                                            );
                                            window.siteimproveHighlightCleanupTimer = null;
                                            window.siteimproveHighlightCleanupFunction = null;
                                        };
                                        window.siteimproveHighlightCleanupTimer = setTimeout(() => {
                                            window.siteimproveHighlightCleanupFunction();
                                        }, 4000);
                                    };

                                    // Highlight text
                                    if (info.offset) {
                                        var originalHTML = element.innerHTML;
                                        var errorChild = element.childNodes[info.offset.child];
                                        var errorText = errorChild.textContent;
                                        var start = info.offset.start;
                                        var end = info.offset.start + info.offset.length;

                                        var beforeWord = errorText.slice(0, start);
                                        var beforeNode = document.createTextNode(beforeWord);
                                        element.insertBefore(beforeNode, errorChild);

                                        var errorWord = errorText.slice(start, end);
                                        var errorNode = document.createElement('span');
                                        errorNode.innerText = errorWord;
                                        errorNode.classList.add(highlightClass);
                                        element.insertBefore(errorNode, errorChild);

                                        var afterWord = errorText.slice(end);
                                        var afterNode = document.createTextNode(afterWord);
                                        element.insertBefore(afterNode, errorChild);

                                        element.removeChild(errorChild);

                                        cleanup(() => {
                                            element.innerHTML = originalHTML;
                                        });
                                    } else {
                                        // Highlight body
                                        if (element.tagName === 'BODY') {
                                            element.classList.add(highlightClassInner);

                                            cleanup(() => {
                                                element.classList.remove(highlightClassInner);
                                            });
                                        } else {
                                            // Highlight other tag types
                                            element.classList.add(highlightClass);

                                            cleanup(() => {
                                                element.classList.remove(highlightClass);
                                            });
                                        }
                                    }
                                }
                            });
                        },
                    ]);

                    self._findAndGenerateAssets(html)
                        .then(() => {
                        // Assets are fetched and pushed to global scope, create package
                        const oZip = new JSzip();

                        self.package.forEach(item => oZip.file(item.filename, item.content));

                        // Genereate package
                        const oLibrary = oZip.generateAsync({
                            type: 'arrayBuffer',
                            compression: 'DEFLATE',
                            compressionOptions: {
                                level: 9
                            }
                        });

                        oLibrary.then((arrayBuffer) => {
                            _si.push(['contentcheck-zip', arrayBuffer, pageUrl, token,
                                function () {
                                }
                            ]);
                        });
                    });
            });
        },
    });
});