json.array!(@builds) do |build|
  json.id build.id
  json.name build.name
  json.branch build.branch
  json.release build.release
end