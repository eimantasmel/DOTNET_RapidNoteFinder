async function getAssociation() {
    return new Promise((resolve, reject) => {
        chrome.storage.sync.get('associate', (result) => {
            resolve(result.associate);
        })
    })
}
export default getAssociation;