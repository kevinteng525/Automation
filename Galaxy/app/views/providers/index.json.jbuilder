json.array!(@providers) do |provider|
  json.extract! provider, :name, :category, :type, :path, :config, :string, :description, :is_active, :integer
  json.url provider_url(provider, format: :json)
end
