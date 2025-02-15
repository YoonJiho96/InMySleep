export function jsonBigIntStringify(obj: any) {
    return JSON.stringify(obj, (key, value) => {
        typeof value === 'bigint' ? value.toString() : value
    })
}