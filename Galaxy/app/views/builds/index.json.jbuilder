json.array!(@builds) do |build|
  json.extract! build, :id, :product_id, :name, :build_type, :status, :branch, :number, :location, :description
  json.url build_url(build, format: :json)
end
